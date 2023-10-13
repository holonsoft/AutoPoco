using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class TimeSpanSourceBase<T>(TimeSpan min, TimeSpan max) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context) {
      var range = (max - min).Ticks;
      var ticks = (long) (Random.NextDouble() * range);
      var result = min.Add(TimeSpan.FromTicks(ticks));

      return (T) (object) result;
   }
}

public class TimeSpanSource(TimeSpan min, TimeSpan max) : TimeSpanSourceBase<TimeSpan>(min, max) {
   public TimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }
}

public class NullableTimeSpanSource(TimeSpan min, TimeSpan max) : TimeSpanSourceBase<TimeSpan?>(min, max) { }