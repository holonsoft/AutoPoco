using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DoubleSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableDoubleListInTermsOfTestability() {
      var source = new DoubleSource(100, 10000, 4);
      var value = source.Next(null);
      value.Should().NotBe(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      var expectedValues = new[] { 9320.4579, 8440.4255, 9606.4981, 9582.5342, 2022.8219, 3929.1042, 3009.8279, 3076.1592, 2897.0263, 7571.7751 };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableDoubleListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDoubleSource(100, 10000, 4);
      var value = source.Next(null);
      value.Should().NotBe(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      var expectedValues = new double?[] {
         3218.163, 9320.4579, 8440.4255, 9606.4981, 9582.5342, 2022.8219, 3929.1042, 3009.8279,
         3076.1592, 2897.0263, null, 7571.7751, null, 602.9881, 8460.9037, 246.5728, null,
         8836.2247, 9351.0288, 8715.8482
      };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}