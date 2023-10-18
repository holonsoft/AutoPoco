using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public abstract class DataSourceBase<T> : IDataSource<T>, IRandomNullEvaluatorSupport {
   public IRandomNullEvaluator RandomNullEvaluator { get; set; } = new DefaultRandomNullEvaluator();

   protected Random Random { get; private set; } = new(AutoPocoGlobalSettings.StandardSeed);

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
