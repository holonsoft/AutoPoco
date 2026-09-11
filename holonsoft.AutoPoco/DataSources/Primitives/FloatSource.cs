using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class FloatSourceBase<T>(float min, float max, int? decimals) : DataSourceBase<T> {
   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public float Min { get; private set; } = min;

   /// <summary>
   ///   Upper bound.
   /// </summary>
   public float Max { get; private set; } = max;
   public int? Decimals { get; private set; } = decimals;

   public FloatSourceBase<T> SetMinMaxAndDecimals(float min, float max, int? decimals) {
      Min = min;
      Max = max;
      Decimals = decimals;
      return this;
   }

   public FloatSourceBase<T> SetMinMax(float min, float max)
      => SetMinMaxAndDecimals(min, max, Decimals);

   public FloatSourceBase<T> SetDecimals(int decimals)
      => SetMinMaxAndDecimals(Min, Max, decimals);

   /// <summary>
   ///   Interpolates between the bounds without overflowing, even over the whole range of the type.
   /// </summary>
   /// <exception cref="ArgumentOutOfRangeException"><see cref="Max" /> is smaller than <see cref="Min" /></exception>
   protected override T GetNextValue(IGenerationContext? context) {
      var value = Random.NextBetween(Min, Max);

      var result = Decimals.HasValue
            ? (float) Math.Round(value, Decimals.Value)
            : value;

      return (T) (object) result;
   }
}

/// <summary>
/// Create a decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
public class FloatSource(float min, float max, int? decimals) : FloatSourceBase<float>(min, max, decimals) {
   public FloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public FloatSource(float min, float max)
      : this(min, max, null) { }

   public FloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }
}

/// <summary>
/// Create a nullable decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults"/>
public class NullableFloatSource(float min, float max, int? decimals) : FloatSourceBase<float?>(min, max, decimals) {
   public NullableFloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public NullableFloatSource(float min, float max)
      : this(min, max, null) { }

   public NullableFloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }
}

