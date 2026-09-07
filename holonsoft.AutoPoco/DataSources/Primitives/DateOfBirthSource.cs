using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class DateOfBirthSourceBase<T>(int minYear, int maxYear) : DataSourceBase<T> {
   public int MinYear { get; private set; } = minYear;
   public int MaxYear { get; private set; } = maxYear;

   public DateOfBirthSourceBase<T> SetMinMaxYears(int minYear, int maxYear) {
      MinYear = minYear;
      MaxYear = maxYear;
      return this;
   }

   /// <summary>
   ///   Picks a day between January 1st of the minimum year and December 31st of the maximum year, both inclusive,
   ///   with every day equally likely.
   /// </summary>
   protected override T GetNextValue(IGenerationContext? context) {
      if (MaxYear < MinYear)
         throw new ArgumentOutOfRangeException(nameof(MaxYear), MaxYear, $"The maximum year must not be before the minimum year ({MinYear}).");

      var firstDay = new DateOnly(MinYear, 1, 1);
      var lastDay = new DateOnly(MaxYear, 12, 31);
      var day = DateOnly.FromDayNumber(firstDay.DayNumber + Random.Next(0, lastDay.DayNumber - firstDay.DayNumber + 1));

      return (T) (object) day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
   }
}

/// <summary>
/// Create a date-of-birth source containing a UTC Datetime with time set to zero
/// </summary>
/// <param name="minYear">Minimum year of birth, default is 1900</param>
/// <param name="maxYear">Maximum year of birth (inclusive), default is 2100 (yes, future date :-) )</param>
public class DateOfBirthSource(int minYear, int maxYear) : DateOfBirthSourceBase<DateTime>(minYear, maxYear) {
   public DateOfBirthSource()
      : this(1900, 2100) { }
}

/// <summary>
/// Create a date-of-birth source containing a UTC Datetime with time set to zero
/// </summary>
/// <param name="minYear">Minimum year of birth, default is 1900</param>
/// <param name="maxYear">Maximum year of birth (inclusive), default is 2100 (yes, future date :-) )</param>
public class NullableDateOfBirthSource(int minYear, int maxYear) : DateOfBirthSourceBase<DateTime?>(minYear, maxYear) {
   public NullableDateOfBirthSource()
      : this(1900, 2100) { }
}
