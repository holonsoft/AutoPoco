using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class IntegerSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new IntegerSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      var expectedValues = new[] { -1809184581, -2056928004, -830599876, 645581134, -1620759081, 1813626657, 1895040361, -1868928598, -998188007, -2043547633 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableIntegerSource();
      var expectedValues = new int?[] { -448584298, null, 676384243, -1809184581, -2056928004, -830599876, 645581134, -1620759081, 1813626657, 1895040361 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}
