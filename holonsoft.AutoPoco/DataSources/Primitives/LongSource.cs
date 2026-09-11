using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class LongSourceBase<T>(long min, long max) : DataSourceBase<T> {
   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public long Min { get; private set; } = min;

   /// <summary>
   ///   Upper bound, inclusive.
   /// </summary>
   public long Max { get; private set; } = max;

   public LongSourceBase<T> SetMinMax(long min, long max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextInclusive(Min, Max);
}

/// <summary>
/// Create a long source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
public class LongSource(long min, long max) : LongSourceBase<long>(min, max) {
   public LongSource()
      : this(long.MinValue, long.MaxValue) { }
}

/// <summary>
/// Create a nullable long source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults"/>
public class NullableLongSource(long min, long max) : LongSourceBase<long?>(min, max) {
   public NullableLongSource()
      : this(long.MinValue, long.MaxValue) { }
}


