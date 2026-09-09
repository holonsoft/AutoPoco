using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class DateTimeSourceBase<T>(DateTime minDate, DateTime maxDate) : DataSourceBase<T> {
   public DateTime MinDate { get; private set; } = minDate;
   public DateTime MaxDate { get; private set; } = maxDate;

   public DateTimeSourceBase<T> SetDateRange(DateTime minDate, DateTime maxDate) {
      MinDate = minDate;
      MaxDate = maxDate;
      return this;
   }

   protected DateOnly GenerateDatePart(DateTime minDate, DateTime maxDate)
      => GenerateDatePart(DateOnly.FromDateTime(minDate), DateOnly.FromDateTime(maxDate));

   /// <summary>
   ///   Picks a day between <paramref name="minDate" /> and <paramref name="maxDate" />, both inclusive,
   ///   with every day of the range equally likely. Month lengths and leap years fall out of the day arithmetic.
   /// </summary>
   protected DateOnly GenerateDatePart(DateOnly minDate, DateOnly maxDate) {
      if (maxDate < minDate)
         throw new ArgumentOutOfRangeException(nameof(maxDate), maxDate, $"The maximum date must not be before the minimum date ({minDate:O}).");

      var days = maxDate.DayNumber - minDate.DayNumber;
      return DateOnly.FromDayNumber(minDate.DayNumber + Random.Next(0, days + 1));
   }

   protected TimeOnly GenerateTimePart(DateTime minTime, DateTime maxTime)
      => GenerateTimePart(TimeOnly.FromDateTime(minTime), TimeOnly.FromDateTime(maxTime));

   /// <summary>
   ///   Picks a time between <paramref name="minTime" /> and <paramref name="maxTime" />, both inclusive, with tick resolution.
   ///   When the maximum lies before the minimum the range wraps around midnight (22:00 to 02:00).
   /// </summary>
   protected TimeOnly GenerateTimePart(TimeOnly minTime, TimeOnly maxTime) {
      var span = maxTime.Ticks - minTime.Ticks;
      if (span < 0)
         span += TimeSpan.TicksPerDay;

      var ticks = (minTime.Ticks + NextTicks(span)) % TimeSpan.TicksPerDay;
      return new TimeOnly(ticks);
   }

   protected override T GetNextValue(IGenerationContext? context) {
      if (MaxDate < MinDate)
         throw new ArgumentOutOfRangeException(nameof(MaxDate), MaxDate, $"The maximum date must not be before the minimum date ({MinDate:O}).");

      var ticks = MinDate.Ticks + NextTicks(MaxDate.Ticks - MinDate.Ticks);
      return (T) (object) new DateTime(ticks, MinDate.Kind);
   }

   /// <summary>
   ///   Random tick count in [0, <paramref name="spanInclusive" />].
   /// </summary>
   private long NextTicks(long spanInclusive)
      => Random.NextInt64(0, spanInclusive + 1);
}

/// <summary>
/// Creates a datetime source that picks any instant between the minimum and the maximum date, both inclusive.
/// The result keeps the <see cref="DateTimeKind" /> of the minimum date.
/// </summary>
/// <param name="minDate">minimum date</param>
/// <param name="maxDate">maximum date</param>
public class DateTimeSource(DateTime minDate, DateTime maxDate) : DateTimeSourceBase<DateTime>(minDate, maxDate) {
   public DateTimeSource()
      : this(DateTime.MinValue, DateTime.MaxValue) { }
}

/// <summary>
/// Creates a nullable datetime source that picks any instant between the minimum and the maximum date, both inclusive.
/// The result keeps the <see cref="DateTimeKind" /> of the minimum date.
/// </summary>
/// <param name="minDate">minimum date</param>
/// <param name="maxDate">maximum date</param>
/// <seealso cref="AutoPocoDefaults"/>
public class NullableDateTimeSource(DateTime minDate, DateTime maxDate) : DateTimeSourceBase<DateTime?>(minDate, maxDate) {
   public NullableDateTimeSource()
      : this(DateTime.MinValue, DateTime.MaxValue) { }
}
