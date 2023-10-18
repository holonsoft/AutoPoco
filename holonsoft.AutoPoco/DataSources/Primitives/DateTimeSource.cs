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

   protected DateOnly GenerateDatePart(DateOnly minDate, DateOnly maxDate) {
      var year = Random.Next(minDate.Year, maxDate.Year);
      var month = Random.Next(1, 12);

      var day = 0;
      switch (month) {
         case 1:
         case 3:
         case 5:
         case 7:
         case 8:
         case 10:
         case 12:
            day = Random.Next(1, 31);
            break;
         case 4:
         case 6:
         case 9:
         case 11:
            day = Random.Next(1, 30);
            break;
         case 2:
            day = DateTime.IsLeapYear(year)
               ? Random.Next(1, 29)
               : Random.Next(1, 28);
            break;
      }

      var result = new DateOnly(year, month, day);

      if (result > maxDate)
         result = result.AddYears(-1);
      if (result < minDate)
         result = result.AddYears(1);

      return result;
   }

   protected TimeOnly GenerateTimePart(DateTime minTime, DateTime maxTime)
      => GenerateTimePart(TimeOnly.FromDateTime(minTime), TimeOnly.FromDateTime(maxTime));

   protected TimeOnly GenerateTimePart(TimeOnly minTime, TimeOnly maxTime) {
      var hour = Random.Next(minTime.Hour, maxTime.Hour);
      var minute = Random.Next(0, 59);
      var second = Random.Next(0, 59);
      var millisecond = Random.Next(0, 999);
      var microsecond = Random.Next(0, 999);
#if NET7_0_OR_GREATER
      var result = new TimeOnly(hour, minute, second, millisecond, microsecond);
#elif NET5_0_OR_GREATER
      var result = new TimeOnly(hour, minute, second, millisecond);
#endif

      if (result < minTime)
         result = minTime;

      if (result >= maxTime)
         result = maxTime;

      return result;
   }

   protected override T GetNextValue(IGenerationContext? context) {
      var dateOnly = GenerateDatePart(MinDate, MaxDate);
      var timeOnly = GenerateTimePart(TimeOnly.MinValue, TimeOnly.MaxValue);
      var result = dateOnly.ToDateTime(timeOnly);

      if (result < MinDate)
         result = MinDate;
      if (result > MaxDate)
         result = MaxDate;

      return (T) (object) result;
   }
}

/// <summary>
/// Creates a datetime source (utc), that is aware about different month length and leap years
/// </summary>
/// <param name="minDate">minimum date</param>
/// <param name="maxDate">maximum date</param>
public class DateTimeSource(DateTime minDate, DateTime maxDate) : DateTimeSourceBase<DateTime>(minDate, maxDate) {
   public DateTimeSource()
      : this(DateTime.MinValue, DateTime.MaxValue) { }
}

/// <summary>
/// Creates a nullable datetime source (utc), that is aware about different month length and leap years
/// </summary>
/// <param name="minDate">minimum date</param>
/// <param name="maxDate">maximum date</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableDateTimeSource(DateTime minDate, DateTime maxDate) : DateTimeSourceBase<DateTime?>(minDate, maxDate) {
   public NullableDateTimeSource()
      : this(DateTime.MinValue, DateTime.MaxValue) { }
}