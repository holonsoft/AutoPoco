using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class DoubleSourceBase<T>(double min, double max, int? decimals) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context) {
      // Perform arithmetic in double type to avoid overflowing
      var range = max - min;
      var sample = Random.NextDouble();

      var result = decimals.HasValue
         ? Math.Round((sample * range) + min, decimals.Value)
         : (sample * range) + min;

      return (T) (object) result;
   }
}

public class DoubleSource(double min, double max, int? decimals) : DoubleSourceBase<double>(min, max, decimals) {
   public DoubleSource()
      : this(double.MinValue, double.MaxValue, null) { }

   public DoubleSource(double min, double max)
      : this(min, max, null) { }

   public DoubleSource(int decimals)
      : this(double.MinValue, double.MaxValue, decimals) { }
}

public class NullableDoubleSource(double min, double max, int? decimals) : DoubleSourceBase<double?>(min, max, decimals) {
   public NullableDoubleSource()
      : this(double.MinValue, double.MaxValue, null) { }

   public NullableDoubleSource(double min, double max)
      : this(min, max, null) { }

   public NullableDoubleSource(int decimals)
      : this(double.MinValue, double.MaxValue, decimals) { }
}