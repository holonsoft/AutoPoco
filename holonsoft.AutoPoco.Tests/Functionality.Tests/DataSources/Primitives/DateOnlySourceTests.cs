using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateOnlySourceTests : TestBase {
   private readonly DateOnly _minDate = new(2000, 1, 1);
   private readonly DateOnly _maxDate = new(2030, 12, 31);

   [Fact]
   public void NextReturnsDateBetweenMinAndMax() {
      var source = new DateOnlySource(_minDate, _maxDate);
      var value = source.Next(null);

      value.Should().BeOnOrAfter(_minDate).And.BeOnOrBefore(_maxDate);
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateOnly[] {
         new DateOnly(2006,2,9),
         new DateOnly(2027,10,29),
         new DateOnly(2028,03,12),
         new DateOnly(2008,4,9),
         new DateOnly(2022,1,26),
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateOnly?[] {
         new DateOnly(2006,2,9),
         null,
         new DateOnly(2027,10,29),
         new DateOnly(2028,03,12),
         new DateOnly(2008,4,9),
      });
   }
}
