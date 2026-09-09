using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class RandomNumberSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new RandomNumberSource(); // same as IntegerSource, exists just for compatibility reasons
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(
         source, new int[] { -1641660861, 161341844, -1702332301, 349857133, -250678351, 1133696339, -2135752200, -515899552, 1177481239, 1035432924 });
   }
}
