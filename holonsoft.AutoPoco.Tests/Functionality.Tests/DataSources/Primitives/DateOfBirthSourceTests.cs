using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateOfBirthSourceTests : TestBase {
   [Fact]
   public void NextReturnsAnAgeBetweenMinAndMax() {
      var source = new DateOfBirthSource(2015, 2018);
      var value = source.Next(null);

      value.Year.Should().BeInRange(2015, 2018);
   }

   
   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateOfBirthSource();
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime[] {
         new (1941, 2, 9, 0, 0, 0, DateTimeKind.Utc),
         new (2086, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new (2091, 3, 12, 0, 0, 0, DateTimeKind.Utc),
         new (1958, 4, 9, 0, 0, 0, DateTimeKind.Utc),
         new (2050, 1, 26, 0, 0, 0, DateTimeKind.Utc),
         new (1902, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new (2074, 1, 14, 0, 0, 0, DateTimeKind.Utc),
         new (2072, 11, 4, 0, 0, 0, DateTimeKind.Utc),
         new (2046, 6, 13, 0, 0, 0, DateTimeKind.Utc),
         new (1920, 10, 26, 0, 0, 0, DateTimeKind.Utc)
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateOfBirthSource();
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime?[] {
         new (1941, 2, 9, 0, 0, 0, DateTimeKind.Utc),
         null,
         new (2086, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new (2091, 3, 12, 0, 0, 0, DateTimeKind.Utc),
         new (1958, 4, 9, 0, 0, 0, DateTimeKind.Utc),
         new (2050, 1, 26, 0, 0, 0, DateTimeKind.Utc),
         new (1902, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new (2074, 1, 14, 0, 0, 0, DateTimeKind.Utc),
         new (2072, 11, 4, 0, 0, 0, DateTimeKind.Utc),
         new (2046, 6, 13, 0, 0, 0, DateTimeKind.Utc),
      });
   }
}
