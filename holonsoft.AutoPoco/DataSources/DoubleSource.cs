using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class DoubleSource(double min, double max, int? decimals) : DataSourceBase<double> {
   public DoubleSource()
      : this(double.MinValue, double.MaxValue, null) { }

   public DoubleSource(double min, double max)
      : this(min, max, null) { }

   public DoubleSource(int decimals)
      : this(double.MinValue, double.MaxValue, decimals) { }

   public override double Next(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = max - min;
      var sample = Random.NextDouble();

      return decimals.HasValue
         ? Math.Round((sample * range) + min, decimals.Value)
         : (sample * range) + min;
   }
}
