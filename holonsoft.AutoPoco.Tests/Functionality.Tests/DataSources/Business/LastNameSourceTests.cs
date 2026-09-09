using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class LastNameSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableLastNamesListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new LastNameSource(), new string[] { "Knight", "Harrison", "Baker", "Diaz", "Harper", "Dean", "Thomas", "Knight", "Long", "Gordon" });

   [Fact]
   public void NextReturnsStableLastNamesListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableLastNameSource()!, new string[] { "Knight", "Harrison", "Baker", "Diaz", "Harper", "Dean", "Thomas", "Knight", "Long", "Gordon" });

}