using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class DateOnlySourceBase<T>(DateOnly minDate, DateOnly maxDate)
      : DateTimeSourceBase<T>(minDate.ToDateTime(TimeOnly.MinValue), maxDate.ToDateTime(TimeOnly.MaxValue)) {

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) GenerateDatePart(minDate, maxDate);
}

public class DateOnlySource(DateOnly minDate, DateOnly maxDate) : DateOnlySourceBase<DateOnly>(minDate, maxDate) {
   public DateOnlySource() : this(DateOnly.MinValue, DateOnly.MaxValue) { }
}

public class NullableDateOnlySource(DateOnly minDate, DateOnly maxDate) : DateOnlySourceBase<DateOnly?>(minDate, maxDate) {
   public NullableDateOnlySource() : this(DateOnly.MinValue, DateOnly.MaxValue) { }
}
