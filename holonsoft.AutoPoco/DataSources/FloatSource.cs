using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class FloatSource(float min, float max, int? decimals) : DataSourceBase<float> {
   public FloatSource()
      : this(float.MinValue, float.MaxValue, null) { }

   public FloatSource(float min, float max)
      : this(min, max, null) { }

   public FloatSource(int decimals)
      : this(float.MinValue, float.MaxValue, decimals) { }

   public override float Next(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) max - min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + min;

      return decimals.HasValue
            ? (float) Math.Round(scaled, decimals.Value)
            : (float) scaled;
   }
}
