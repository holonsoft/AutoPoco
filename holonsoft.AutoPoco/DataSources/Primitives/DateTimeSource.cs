using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public class DateTimeSource(DateTime minDate, DateTime maxDate) : DataSourceBase<DateTime>
{
    public DateTimeSource()
       : this(DateTime.MinValue, DateTime.MaxValue) { }

    public override DateTime Next(IGenerationContext? context)
    {
        var range = (maxDate - minDate).Ticks;
        var ticks = (long)(Random.NextDouble() * range);
        return minDate.AddTicks(ticks);
    }
}
