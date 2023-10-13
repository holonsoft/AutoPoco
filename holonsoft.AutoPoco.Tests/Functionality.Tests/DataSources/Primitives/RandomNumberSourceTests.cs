using FluentAssertions;
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

      value2.Should().NotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(
         source,
         -1809184581, -2056928004, -830599876, 645581134, -1620759081, 1813626657, 1895040361, -1868928598, -998188007, -2043547633
         );
   }
}
