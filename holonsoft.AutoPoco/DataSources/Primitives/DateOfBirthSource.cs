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

   protected override T GetNextValue(IGenerationContext? context) {
      var year = Random.Next(MinYear, MaxYear);
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

      return (T) (object) new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
   }
}

/// <summary>
/// Create a date-of-birth source containing a UTC Datetime with time set to zero
/// </summary>
/// <param name="minYear">Minimum year of birth, default is 1900</param>
/// <param name="maxYear">Maximum year of birth, default is 2100 (yes, future date :-) )</param>
public class DateOfBirthSource(int minYear, int maxYear) : DateOfBirthSourceBase<DateTime>(minYear, maxYear) {
   public DateOfBirthSource()
      : this(1900, 2100) { }
}

/// <summary>
/// Create a date-of-birth source containing a UTC Datetime with time set to zero
/// </summary>
/// <param name="minYear">Minimum year of birth, default is 1900</param>
/// <param name="maxYear">Maximum year of birth, default is 2100 (yes, future date :-) )</param>
public class NullableDateOfBirthSource(int minYear, int maxYear) : DateOfBirthSourceBase<DateTime?>(minYear, maxYear) {
   public NullableDateOfBirthSource()
      : this(1900, 2100) { }
}