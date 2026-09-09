using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeSpanSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestability() {
      var source = new TimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan[] { new TimeSpan(5550259682), new TimeSpan(8617400021), new TimeSpan(-6415501310), new TimeSpan(3090046013), new TimeSpan(-8011301711), new TimeSpan(-4787452734), new TimeSpan(-5434663020), new TimeSpan(6384537375), new TimeSpan(8346920384), new TimeSpan(-100027733) });
   }

   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan?[] { new TimeSpan(3518868239), new TimeSpan(6289472960), new TimeSpan(5550259682), new TimeSpan(8617400021), new TimeSpan(-6415501310), new TimeSpan(3090046013), new TimeSpan(-8011301711), new TimeSpan(-4787452734), new TimeSpan(-5434663020), new TimeSpan(6384537375) });
   }
}