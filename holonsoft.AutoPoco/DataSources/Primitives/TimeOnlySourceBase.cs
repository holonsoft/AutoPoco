using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class TimeOnlySourceBase<T>(TimeOnly minTime, TimeOnly maxTime)
   : DateTimeSourceBase<T>(DateOnly.MinValue.ToDateTime(minTime), DateOnly.MinValue.ToDateTime(maxTime)) {

   public TimeOnly MinTime { get; private set; } = minTime;
   public TimeOnly MaxTime { get; private set; } = maxTime;

   public TimeOnlySourceBase<T> SetMinMaxRange(TimeOnly minTime, TimeOnly maxTime) {
      MinTime = minTime;
      MaxTime = maxTime;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) GenerateTimePart(MinTime, MaxTime);
}

/// <summary>
/// Create a timeonly source
/// </summary>
/// <param name="minTime"></param>
/// <param name="maxTime"></param>
public class TimeOnlySource(TimeOnly minTime, TimeOnly maxTime) : TimeOnlySourceBase<TimeOnly>(minTime, maxTime) {
   public TimeOnlySource() : this(TimeOnly.MinValue, TimeOnly.MaxValue) { }
}

/// <summary>
/// Create a nullable timeonly source
/// </summary>
/// <param name="minTime"></param>
/// <param name="maxTime"></param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableTimeOnlySource(TimeOnly minTime, TimeOnly maxTime) : TimeOnlySourceBase<TimeOnly?>(minTime, maxTime) {
   public NullableTimeOnlySource() : this(TimeOnly.MinValue, TimeOnly.MaxValue) { }
}

