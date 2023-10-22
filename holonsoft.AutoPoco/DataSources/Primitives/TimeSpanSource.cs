using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class TimeSpanSourceBase<T>(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : DataSourceBase<T> {

   public TimeSpan MinTimeSpan { get; private set; } = minTimeSpan;
   public TimeSpan MaxTimeSpan { get; private set; } = maxTimeSpan;

   public TimeSpanSourceBase<T> SetMinMaxRange(TimeSpan min, TimeSpan max) {
      MinTimeSpan = min;
      MaxTimeSpan = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context) {
      var range = (MaxTimeSpan - MinTimeSpan).Ticks;
      var ticks = (long) (Random.NextDouble() * range);

      if (ticks < TimeSpan.MinValue.Ticks)
         ticks = TimeSpan.MinValue.Ticks;

      if (ticks > TimeSpan.MaxValue.Ticks) 
         ticks = TimeSpan.MaxValue.Ticks;

      while (ticks < MinTimeSpan.Ticks)
         ticks *= Random.Next(1, 3);

      while ((MinTimeSpan.Ticks + ticks) > MaxTimeSpan.Ticks)
         ticks /= Random.Next(2, 4);

      var result = MinTimeSpan.Add(TimeSpan.FromTicks(ticks));

      return (T) (object) result;
   }
}

/// <summary>
/// Create a timespan source
/// </summary>
/// <param name="minTimeSpan">Minimum value</param>
/// <param name="maxTimeSpan">maximum value</param>
public class TimeSpanSource(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : TimeSpanSourceBase<TimeSpan>(minTimeSpan, maxTimeSpan) {
   public TimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }
}

/// <summary>
/// Create a timespan source
/// </summary>
/// <param name="minTimeSpan">Minimum value</param>
/// <param name="maxTimeSpan">maximum value</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableTimeSpanSource(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : TimeSpanSourceBase<TimeSpan?>(minTimeSpan, maxTimeSpan)
{
   public NullableTimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }
}