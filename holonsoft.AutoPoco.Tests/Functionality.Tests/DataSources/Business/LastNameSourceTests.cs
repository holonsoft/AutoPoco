using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class LastNameSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableLastNamesListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new LastNameSource(),
         "Turner", "Martin", "Murphy", "Pierce", "Wells", "Scott", "Scott", "Baker", "Perez", "Evans");

   [Fact]
   public void NextReturnsStableLastNamesListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableLastNameSource()!,
         "Turner", "Martin", "Murphy", "Pierce", "Wells", "Scott", "Scott", "Baker", "Perez", "Evans");

}