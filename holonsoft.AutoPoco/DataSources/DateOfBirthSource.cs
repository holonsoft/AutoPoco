// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateOfBirthSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The date of birth source.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="DateOfBirthSource" /> class.
/// </remarks>
/// <param name="yearsMin">
///   The min.
/// </param>
/// <param name="yearsMax">
///   The max.
/// </param>
public class DateOfBirthSource(int yearsMin, int yearsMax) : DataSourceBase<DateTime> {
   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The <see cref="DateTime" />.
   /// </returns>
   public override DateTime Next(IGenerationContext? context) {
      var year = DateTime.Now.Year - RandomNumberGenerator.Current.Next(yearsMin, yearsMax);
      var month = RandomNumberGenerator.Current.Next(1, 12);

      var day = 0;
      switch (month) {
         case 1:
         case 3:
         case 5:
         case 7:
         case 8:
         case 10:
         case 12:
            day = RandomNumberGenerator.Current.Next(1, 31);
            break;
         case 4:
         case 6:
         case 9:
         case 11:
            day = RandomNumberGenerator.Current.Next(1, 30);
            break;
         case 2:
            day = DateTime.IsLeapYear(year)
               ? RandomNumberGenerator.Current.Next(1, 29)
               : RandomNumberGenerator.Current.Next(1, 28);
            break;
      }

      return new DateTime(year, month, day);
   }
}