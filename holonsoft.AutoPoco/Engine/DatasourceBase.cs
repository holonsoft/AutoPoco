using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

public abstract class DataSourceBase<T> : IDataSource<T>, IRandomNullEvaluatorSupport, ISessionSeedable {
   private bool _hasExplicitSeed;
   private bool _sessionSeedApplied;

   public IRandomNullEvaluator RandomNullEvaluator { get; set; } = new DefaultRandomNullEvaluator();

   /// <summary>
   ///   The random stream of this source, a <see cref="StableRandom" /> seeded with <see cref="AutoPocoDefaults.Seed" />
   ///   until the engine or <see cref="SetSeedToRandomValue(int)" /> reseeds it.
   /// </summary>
   protected Random Random { get; private set; } = new StableRandom(AutoPocoDefaults.Seed);

   protected DataSourceBase() { }

   /// <summary>
   ///   Creates the source with an explicit null creation threshold (percent).
   ///   A null threshold keeps the default evaluator, see <see cref="AutoPocoDefaults.NullCreationThreshold" />.
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

   /// <summary>
   ///   Reseeds with a value that differs on every call. The source is no longer repeatable afterwards.
   /// </summary>
   public virtual void SetSeedToRandomValue()
      => SetSeedToRandomValue(Guid.NewGuid().GetHashCode());

   /// <summary>
   ///   Reseeds the random stream and the null evaluator. An explicit seed wins over the session seed of the engine.
   /// </summary>
   public virtual void SetSeedToRandomValue(int seed) {
      Random = new StableRandom(seed);
      RandomNullEvaluator.SetSeedToRandomValue(seed);
      _hasExplicitSeed = true;
   }

   /// <summary>
   ///   Called by the engine with the seed derived for the member this source feeds. Applied once,
   ///   and only when no explicit seed was set.
   /// </summary>
   void ISessionSeedable.ApplySessionSeed(int seed) {
      if (_hasExplicitSeed || _sessionSeedApplied)
         return;

      _sessionSeedApplied = true;
      SetSeedToRandomValue(seed);
      _hasExplicitSeed = false;
   }

   public DataSourceBase<T> SetNullCreationThreshold(int nullCreationThreshold) {
      RandomNullEvaluator = new DefaultRandomNullEvaluator(nullCreationThreshold);
      return this;
   }

   public void SetRandomNullEvaluator(IRandomNullEvaluator randomNullEvaluator) {
      ArgumentNullException.ThrowIfNull(randomNullEvaluator);
      RandomNullEvaluator = randomNullEvaluator;
   }

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
