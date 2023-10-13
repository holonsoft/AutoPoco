using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class IntegerSourceBase<T>(int min, int max) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.Next(min, max);
}

public class IntegerSource(int min, int max) : IntegerSourceBase<int>(min, max) {
   public IntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

public class NullableIntegerSource(int min, int max) : IntegerSourceBase<int?>(min, max) {
   public NullableIntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

