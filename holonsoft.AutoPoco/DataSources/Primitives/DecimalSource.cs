using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class DecimalSourceBase<T>(decimal min, decimal max, int? decimals) : DataSourceBase<T> {

   public decimal Min { get; private set; } = min;
   public decimal Max { get; private set; } = max;
   public int? Decimals { get; private set; } = decimals;

   public DecimalSourceBase<T> SetMinMaxAndDecimals(decimal min, decimal max, int? decimals) {
      Min = min;
      Max = max;
      Decimals = decimals;
      return this;
   }

   public DecimalSourceBase<T> SetMinMax(decimal min, decimal max)
      => SetMinMaxAndDecimals(min, max, Decimals);

   public DecimalSourceBase<T> SetDecimals(int decimals)
      => SetMinMaxAndDecimals(Min, Max, decimals);

   protected override T GetNextValue(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = (double) Max - (double) Min;
      var sample = Random.NextDouble();
      var scaled = (sample * range) + (double) Min;

      var result = Decimals.HasValue
         ? Math.Round((decimal) scaled, Decimals.Value)
         : (decimal) scaled;

      return (T) (object) result;
   }
}

/// <summary>
/// Create a decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
public class DecimalSource(decimal min, decimal max, int? decimals = null) : DecimalSourceBase<decimal>(min, max, decimals) {
   public DecimalSource()
      : this(decimal.MinValue, decimal.MaxValue, null) { }
}

/// <summary>
/// Create a nullable decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableDecimalSource(decimal min, decimal max, int? decimals = null) : DecimalSourceBase<decimal?>(min, max, decimals) {
   public NullableDecimalSource()
      : this(decimal.MinValue, decimal.MaxValue, null) { }
}

