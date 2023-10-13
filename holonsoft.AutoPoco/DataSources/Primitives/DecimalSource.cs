using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class DecimalSourceBase<T>(decimal min, decimal max, int? decimals) : DataSourceBase<T> {

   protected override T GetNextValue(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) max - (double) min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + (double) min;

      var result = decimals.HasValue
         ? Math.Round((decimal) scaled, decimals.Value)
         : (decimal) scaled;

      return (T) (object) result;
   }
}

public class DecimalSource(decimal min, decimal max, int? decimals = null) : DecimalSourceBase<decimal>(min, max, decimals) {
   public DecimalSource()
      : this(decimal.MinValue, decimal.MaxValue, null) { }
}

public class NullableDecimalSource(decimal min, decimal max, int? decimals = null) : DecimalSourceBase<decimal?>(min, max, decimals) {
   public NullableDecimalSource()
      : this(decimal.MinValue, decimal.MaxValue, null) { }
}
