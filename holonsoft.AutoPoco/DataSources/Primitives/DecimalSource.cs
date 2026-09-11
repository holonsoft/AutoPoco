using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class DecimalSourceBase<T>(decimal min, decimal max, int? decimals) : DataSourceBase<T> {

   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public decimal Min { get; private set; } = min;

   /// <summary>
   ///   Upper bound.
   /// </summary>
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

   /// <summary>
   ///   Interpolates between the bounds in decimal arithmetic. Does not overflow, even over the whole
   ///   range of the type, and keeps the precision that the detour through double used to lose.
   /// </summary>
   /// <exception cref="ArgumentOutOfRangeException"><see cref="Max" /> is smaller than <see cref="Min" /></exception>
   protected override T GetNextValue(IGenerationContext? context) {
      var value = Random.NextBetween(Min, Max);

      var result = Decimals.HasValue
         ? Math.Round(value, Decimals.Value)
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
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults"/>
public class NullableDecimalSource(decimal min, decimal max, int? decimals = null) : DecimalSourceBase<decimal?>(min, max, decimals) {
   public NullableDecimalSource()
      : this(decimal.MinValue, decimal.MaxValue, null) { }
}

