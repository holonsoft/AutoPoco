using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class TimeSpanSource(TimeSpan min, TimeSpan max) : DataSourceBase<TimeSpan> {

   public TimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }

   public override TimeSpan Next(IGenerationContext? context) {
      var range = (max - min).Ticks;
      var ticks = (long) (Random.NextDouble() * range);
      return min.Add(TimeSpan.FromTicks(ticks));
   }
}
