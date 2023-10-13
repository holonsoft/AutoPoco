using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class FloatSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableFloatListInTermsOfTestability() {
      var source = new FloatSource(3);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      var expectedValues = new[] { -1.2592764E+38F, 2.9356796E+38F, 2.330711E+38F, 3.132315E+38F, 3.1158412E+38F, -2.0810006E+38F, -7.7054755E+37F, -1.402494E+38F, -1.3568953E+38F, -1.4800383E+38F };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableFloatListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableFloatSource(3);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      var expectedValues = new float?[] {
         -2.5996409E+38F, -1.2592764E+38F, 2.9356796E+38F, 2.330711E+38F, 3.132315E+38F, 3.1158412E+38F, -2.0810006E+38F, -7.7054755E+37F,
         -1.402494E+38F, -1.3568953E+38F, -1.4800383E+38F, null, 1.7335668E+38F, null, -3.0570498E+38F, 2.3447885E+38F, -3.3020635E+38F,
         null, 2.6027988E+38F, 2.9566952E+38F,
      };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}