using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class DoubleSourceBase<T>(double min, double max, int? decimals) : DataSourceBase<T> {
   public double Min { get; private set; } = min;
   public double Max { get; private set; } = max;
   public int? Decimals { get; private set; } = decimals;

   public DoubleSourceBase<T> SetMinMaxAndDecimals(double min, double max, int? decimals) {
      Min = min;
      Max = max;
      Decimals = decimals;
      return this;
   }

   public DoubleSourceBase<T> SetMinMax(double min, double max)
      => SetMinMaxAndDecimals(min, max, Decimals);

   public DoubleSourceBase<T> SetDecimals(int decimals)
      => SetMinMaxAndDecimals(Min, Max, decimals);

   protected override T GetNextValue(IGenerationContext? context) {

      var range = Max - Min;
      var sample = Random.NextDouble();

      var result = Decimals.HasValue
         ? Math.Round((sample * range) + Min, Decimals.Value)
         : (sample * range) + Min;

      return (T) (object) result;
   }
}

/// <summary>
/// Create a decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
public class DoubleSource(double min, double max, int? decimals) : DoubleSourceBase<double>(min, max, decimals) {
   public DoubleSource()
      : this(double.MinValue, double.MaxValue, null) { }

   public DoubleSource(double min, double max)
      : this(min, max, null) { }

   public DoubleSource(int decimals)
      : this(double.MinValue, double.MaxValue, decimals) { }
}

/// <summary>
/// Create a decimal source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <param name="decimals">Count of decimals</param>
public class NullableDoubleSource(double min, double max, int? decimals) : DoubleSourceBase<double?>(min, max, decimals) {
   public NullableDoubleSource()
      : this(double.MinValue, double.MaxValue, null) { }

   public NullableDoubleSource(double min, double max)
      : this(min, max, null) { }

   public NullableDoubleSource(int decimals)
      : this(double.MinValue, double.MaxValue, decimals) { }
}