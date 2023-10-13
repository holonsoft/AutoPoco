using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class FloatSourceBase<T>(float min, float max, int? decimals) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) max - min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + min;

      var result = decimals.HasValue
            ? (float) Math.Round(scaled, decimals.Value)
            : (float) scaled;

      return (T) (object) result;
   }
}

public class FloatSource(float min, float max, int? decimals) : FloatSourceBase<float>(min, max, decimals) {
   public FloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public FloatSource(float min, float max)
      : this(min, max, null) { }

   public FloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }
}

public class NullableFloatSource(float min, float max, int? decimals) : FloatSourceBase<float?>(min, max, decimals) {
   public NullableFloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public NullableFloatSource(float min, float max)
      : this(min, max, null) { }

   public NullableFloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }
}

