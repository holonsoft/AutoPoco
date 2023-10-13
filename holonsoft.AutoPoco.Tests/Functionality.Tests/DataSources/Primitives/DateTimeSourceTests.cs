using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateTimeSourceTests : TestBase {
   private readonly DateTime _minDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   private readonly DateTime _maxDate = new(2030, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

   [Fact]
   public void NextReturnsDateTimeBetweenMinAndMax() {
      var source = new DateTimeSource(_minDate, _maxDate);
      var value = source.Next(null);

      value.Should().BeOnOrAfter(_minDate).And.BeOnOrBefore(_maxDate);
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateTimeSource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime[] {
         new DateTime(632751185969561940, DateTimeKind.Utc),
         new DateTime(634379282428430140, DateTimeKind.Utc),
         new DateTime(639312496709501070, DateTimeKind.Utc),
         new DateTime(637906853897766270, DateTimeKind.Utc),
         new DateTime(639802962923061100, DateTimeKind.Utc)
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateTimeSource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability<DateTime?>(source, new DateTime?[] {
         new DateTime(632751185969561940, DateTimeKind.Utc),
         null,
         new DateTime(634379282428430140, DateTimeKind.Utc),
         new DateTime(639312496709501070, DateTimeKind.Utc),
         new DateTime(637906853897766270, DateTimeKind.Utc)
      });
   }
}
