// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateOfBirthSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class DateOfBirthSourceBase<T>(int yearsMin, int yearsMax) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context) {
      var year = Random.Next(yearsMin, yearsMax);
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

public class DateOfBirthSource(int yearsMin, int yearsMax) : DateOfBirthSourceBase<DateTime>(yearsMin, yearsMax) {
   public DateOfBirthSource()
      : this(1900, DateTime.UtcNow.Year) { }
}

public class NullableDateOfBirthSource(int yearsMin, int yearsMax) : DateOfBirthSourceBase<DateTime?>(yearsMin, yearsMax) {
   public NullableDateOfBirthSource()
      : this(1900, DateTime.UtcNow.Year) { }
}