using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeOnlySourceTests : TestBase {
   private readonly TimeOnly _minDate = new(8, 0, 0);
   private readonly TimeOnly _maxDate = new(17, 0, 0);

   [Fact]
   public void NextReturnsDateBetweenMinAndMax() {
      var source = new TimeOnlySource(_minDate, _maxDate);
      var value = source.Next(null);

      value.Should().BeOnOrAfter(_minDate).And.BeOnOrBefore(_maxDate);
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new TimeOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new TimeOnly[] {
         new TimeOnly(9,6,18,930,841),
         new TimeOnly(16,56,11,386,293),
         new TimeOnly(10,16,44,50,843),
         new TimeOnly(8,52,55,869,7),
         new TimeOnly(12,50,56,107,732),
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new TimeOnly?[] {
         new TimeOnly(9,6,18,930,841),
         null,
         new TimeOnly(16,56,11,386,293),
         new TimeOnly(10,16,44,50,843),
         new TimeOnly(8,52,55,869,7),
      });
   }
}