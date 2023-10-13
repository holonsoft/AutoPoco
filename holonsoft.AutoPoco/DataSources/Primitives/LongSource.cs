using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class LongSourceBase<T>(long min, long max) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextInt64(min, max);
}

public class LongSource(long min, long max) : LongSourceBase<long>(min, max) {
   public LongSource()
      : this(long.MinValue, long.MaxValue) { }
}

public class NullableLongSource(long min, long max) : LongSourceBase<long?>(min, max) {
   public NullableLongSource()
      : this(long.MinValue, long.MaxValue) { }
}
