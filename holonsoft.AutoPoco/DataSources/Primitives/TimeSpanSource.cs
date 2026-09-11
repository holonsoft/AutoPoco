using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class TimeSpanSourceBase<T>(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : DataSourceBase<T> {

   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public TimeSpan MinTimeSpan { get; private set; } = minTimeSpan;

   /// <summary>
   ///   Upper bound, inclusive.
   /// </summary>
   public TimeSpan MaxTimeSpan { get; private set; } = maxTimeSpan;

   public TimeSpanSourceBase<T> SetMinMaxRange(TimeSpan min, TimeSpan max) {
      MinTimeSpan = min;
      MaxTimeSpan = max;
      return this;
   }

   /// <summary>
   ///   Uniform pick from the inclusive tick range, the same way the date and time sources work.
   /// </summary>
   /// <exception cref="ArgumentOutOfRangeException">the maximum lies before the minimum</exception>
   protected override T GetNextValue(IGenerationContext? context) {
      // checked here as well, so the message names time spans instead of raw ticks
      if (MaxTimeSpan < MinTimeSpan)
         throw new ArgumentOutOfRangeException(nameof(MaxTimeSpan), MaxTimeSpan, $"The maximum must not be smaller than the minimum {MinTimeSpan}.");

      return (T) (object) TimeSpan.FromTicks(Random.NextInclusive(MinTimeSpan.Ticks, MaxTimeSpan.Ticks));
   }
}

/// <summary>
/// Create a timespan source
/// </summary>
/// <param name="minTimeSpan">Minimum value, inclusive</param>
/// <param name="maxTimeSpan">Maximum value, inclusive</param>
public class TimeSpanSource(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : TimeSpanSourceBase<TimeSpan>(minTimeSpan, maxTimeSpan) {
   public TimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }
}

/// <summary>
/// Create a timespan source
/// </summary>
/// <param name="minTimeSpan">Minimum value, inclusive</param>
/// <param name="maxTimeSpan">Maximum value, inclusive</param>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults"/>
public class NullableTimeSpanSource(TimeSpan minTimeSpan, TimeSpan maxTimeSpan) : TimeSpanSourceBase<TimeSpan?>(minTimeSpan, maxTimeSpan)
{
   public NullableTimeSpanSource()
      : this(TimeSpan.MinValue, TimeSpan.MaxValue) { }
}