using Shouldly;
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

      value2.ShouldNotBe(value1);

      var expectedValues = new int[] { -1641660861, 161341844, -1702332301, 349857133, -250678351, 1133696339, -2135752200, -515899552, 1177481239, 1035432924 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableIntegerSource();
      var expectedValues = new int?[] { -813551908, 1317559791, -1641660861, 161341844, -1702332301, 349857133, -250678351, 1133696339, -2135752200, -515899552 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}
