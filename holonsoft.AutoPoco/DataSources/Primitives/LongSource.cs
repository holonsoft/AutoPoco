using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class LongSourceBase<T>(long min, long max) : DataSourceBase<T> {
   public long Min { get; private set; } = min;
   public long Max { get; private set; } = max;

   public LongSourceBase<T> SetMinMax(int min, int max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextInt64(Min, Max);
}

/// <summary>
/// Create an integer source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
public class LongSource(long min, long max) : LongSourceBase<long>(min, max) {
   public LongSource()
      : this(long.MinValue, long.MaxValue) { }
}

/// <summary>
/// Create a nullable integer source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableLongSource(long min, long max) : LongSourceBase<long?>(min, max) {
   public NullableLongSource()
      : this(long.MinValue, long.MaxValue) { }
}


