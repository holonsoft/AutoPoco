using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;

#if NET7_0_OR_GREATER
public abstract class Int128SourceBase<T>(Int128 min, Int128 max) : DataSourceBase<T> {
   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public Int128 Min { get; private set; } = min;

   /// <summary>
   ///   Upper bound, inclusive.
   /// </summary>
   public Int128 Max { get; private set; } = max;

   public Int128SourceBase<T> SetMinMax(Int128 min, Int128 max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextInclusive(Min, Max);
}

/// <summary>
/// Create an Int128 source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
public class Int128Source(Int128 min, Int128 max) : Int128SourceBase<Int128>(min, max) {
   public Int128Source()
      : this(Int128.MinValue, Int128.MaxValue) { }
}

/// <summary>
/// Create a nullable Int128 source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
/// <seealso cref="AutoPocoDefaults"/>
public class NullableInt128Source(Int128 min, Int128 max) : Int128SourceBase<Int128?>(min, max) {
   public NullableInt128Source()
      : this(Int128.MinValue, Int128.MaxValue) { }
}

#endif