using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class DateOnlySourceBase<T>(DateOnly minDate, DateOnly maxDate)
   : DateTimeSourceBase<T>(minDate.ToDateTime(TimeOnly.MinValue), maxDate.ToDateTime(TimeOnly.MaxValue)) {

   public DateOnly MinDateOnly { get; private set; } = minDate;
   public DateOnly MaxDateOnly { get; private set; } = maxDate;

   public DateOnlySourceBase<T> SetDateRange(DateOnly minDate, DateOnly maxDate) {
      MinDateOnly = minDate;
      MaxDateOnly = maxDate;
      return this;
   }

   public DateOnlySourceBase<T> SetMinDate(DateOnly minDate) {
      MinDateOnly = minDate;
      return this;
   }

   public DateOnlySourceBase<T> SetMaxDate(DateOnly maxDate) {
      MaxDateOnly = maxDate;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) GenerateDatePart(MinDateOnly, MaxDateOnly);
}

/// <summary>
/// Create a DateOnly source, that is aware about different month length and leap years
/// </summary>
/// <param name="minDate">Minimum date</param>
/// <param name="maxDate">Maximum date</param>
public class DateOnlySource(DateOnly minDate, DateOnly maxDate) : DateOnlySourceBase<DateOnly>(minDate, maxDate) {
   public DateOnlySource() : this(DateOnly.MinValue, DateOnly.MaxValue) { }
}

/// <summary>
/// Create a DateOnly source, that is aware about different month length and leap years
/// </summary>
/// <param name="minDate">Minimum date</param>
/// <param name="maxDate">Maximum date</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableDateOnlySource(DateOnly minDate, DateOnly maxDate) : DateOnlySourceBase<DateOnly?>(minDate, maxDate) {
   public NullableDateOnlySource() : this(DateOnly.MinValue, DateOnly.MaxValue) { }
}
