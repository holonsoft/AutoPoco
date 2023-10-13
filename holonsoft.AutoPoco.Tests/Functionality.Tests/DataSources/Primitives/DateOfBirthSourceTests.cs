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
         new(1925, 2, 9, 0,0,0,DateTimeKind.Utc),
         new(2014, 10, 29,0,0,0, DateTimeKind.Utc),
         new(2017, 3, 12, 0, 0, 0, DateTimeKind.Utc),
         new(1936, 4, 9, 0, 0, 0, DateTimeKind.Utc),
         new(1992, 1, 26, 0, 0, 0, DateTimeKind.Utc),
         new(1901, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new(2007, 1, 14, 0, 0, 0, DateTimeKind.Utc),
         new(2006, 11, 4, 0, 0, 0, DateTimeKind.Utc),
         new(1990, 6, 13, 0, 0, 0, DateTimeKind.Utc),
         new(1912, 10, 26, 0, 0, 0, DateTimeKind.Utc)
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateOfBirthSource();
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime?[] {
         new(1925, 2, 9, 0, 0, 0, DateTimeKind.Utc),
         null,
         new(2014, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new(2017, 3, 12, 0, 0, 0, DateTimeKind.Utc),
         new(1936, 4, 9, 0, 0, 0, DateTimeKind.Utc),
         new(1992, 1, 26, 0, 0, 0, DateTimeKind.Utc),
         new(1901, 10, 29, 0, 0, 0, DateTimeKind.Utc),
         new(2007, 1, 14, 0, 0, 0, DateTimeKind.Utc),
         new(2006, 11, 4, 0, 0, 0, DateTimeKind.Utc),
         new(1990, 6, 13, 0, 0, 0, DateTimeKind.Utc)
      });
   }
}
