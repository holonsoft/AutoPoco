using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class TimeOnlySourceBase<T>(TimeOnly minTime, TimeOnly maxTime)
      : DateTimeSourceBase<T>(DateOnly.MinValue.ToDateTime(minTime), DateOnly.MinValue.ToDateTime(maxTime)) {

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) GenerateTimePart(minTime, maxTime);
}

public class TimeOnlySource(TimeOnly minTime, TimeOnly maxTime) : TimeOnlySourceBase<TimeOnly>(minTime, maxTime) {
   public TimeOnlySource() : this(TimeOnly.MinValue, TimeOnly.MaxValue) { }
}

public class NullableTimeOnlySource(TimeOnly minTime, TimeOnly maxTime) : TimeOnlySourceBase<TimeOnly?>(minTime, maxTime) {
   public NullableTimeOnlySource() : this(TimeOnly.MinValue, TimeOnly.MaxValue) { }
}

