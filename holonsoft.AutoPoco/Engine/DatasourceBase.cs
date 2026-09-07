using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public abstract class DataSourceBase<T> : IDataSource<T>, IRandomNullEvaluatorSupport {
   public IRandomNullEvaluator RandomNullEvaluator { get; set; } = new DefaultRandomNullEvaluator();

   protected Random Random { get; private set; } = new(AutoPocoGlobalSettings.StandardSeed);

   protected DataSourceBase() { }

   /// <summary>
   ///   Creates the source with an explicit null creation threshold (percent).
   ///   A null threshold keeps the default evaluator, see <see cref="AutoPocoGlobalSettings.NullCreationThreshold" />.
   /// </summary>
   protected DataSourceBase(int? nullCreationThreshold) {
      NullCreationThreshold = nullCreationThreshold;
      if (nullCreationThreshold.HasValue)
         RandomNullEvaluator = new DefaultRandomNullEvaluator(nullCreationThreshold.Value);
   }

   /// <summary>
   ///   The explicit null creation threshold given at construction, null when the source was created without one.
   ///   Reference type sources use it to decide whether they produce nulls at all.
   /// </summary>
   protected int? NullCreationThreshold { get; }

   public virtual void SetSeedToRandomValue() {
      var seed = Guid.NewGuid().GetHashCode();
      Random = new(seed);
      RandomNullEvaluator.SetSeedToRandomValue(seed);
   }

   public virtual void SetSeedToRandomValue(int seed) {
      Random = new(seed);
      RandomNullEvaluator.SetSeedToRandomValue(seed);
   }

   public DataSourceBase<T> SetNullCreationThreshold(int nullCreationThreshold) {
      RandomNullEvaluator = new DefaultRandomNullEvaluator(nullCreationThreshold);
      return this;
   }

   public void SetRandomNullEvaluator(IRandomNullEvaluator randomNullEvaluator)
      => RandomNullEvaluator = randomNullEvaluator;

   object? IDataSource.InternalNext(IGenerationContext? context) =>
      Next(context)!;

   /// <summary>
   ///   Gets the next object from this data source
   /// </summary>
   /// <returns>next value</returns>
   protected abstract T GetNextValue(IGenerationContext? context);

   public T Next(IGenerationContext? context) {
      var type = typeof(T);
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return default!;

      return GetNextValue(context)!;
   }
}
