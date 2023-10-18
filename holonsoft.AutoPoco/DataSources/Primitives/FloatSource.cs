using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class FloatSourceBase<T>(float min, float max, int? decimals) : DataSourceBase<T> {
   public float Min { get; private set; } = min;
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

   protected override T GetNextValue(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) Max - Min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + Min;

      var result = Decimals.HasValue
            ? (float) Math.Round(scaled, Decimals.Value)
            : (float) scaled;
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
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableFloatSource(float min, float max, int? decimals) : FloatSourceBase<float?>(min, max, decimals) {
   public NullableFloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public NullableFloatSource(float min, float max)
      : this(min, max, null) { }

   public NullableFloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }
}

