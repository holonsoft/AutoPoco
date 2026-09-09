using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class BooleanSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestability() {
      var source = new BooleanSource();
      NextReturnsStableElementListInTermsOfTestability(source, new bool[] { false, true, true, false, true, true, true, true, false, false });
   }

   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableBooleanSource();
      NextReturnsStableElementListInTermsOfTestability(source, new bool?[] { false, true, true, false, true, true, true, true, false, false });
   }
}
