using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class DecimalSource(decimal min, decimal max, int? decimals) : DataSourceBase<decimal> {
   public DecimalSource() : this(decimal.MinValue, decimal.MaxValue, null) {
   }

   public DecimalSource(decimal min, decimal max) : this(min, max, null) {
   }

   public override decimal Next(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) max - (double) min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + (double) min;

      return decimals.HasValue
         ? Math.Round((decimal) scaled, decimals.Value)
         : (decimal) scaled;
   }
}
