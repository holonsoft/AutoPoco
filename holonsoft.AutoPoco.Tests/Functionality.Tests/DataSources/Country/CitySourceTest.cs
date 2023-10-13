using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CitySourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableCityListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CitySource(),
         "Lima", "Lahore", "Rio de Janeiro", "Monaco", "Ulaanbaatar", "Basseterre", "Basseterre", "Mexico City", "Guangzhou", "Chennai", "Taipei", "Kuala Lumpur",
         "Tripoli", "Dhaka", "Ulaanbaatar", "Beijing", "Asmara", "Vatican City", "Hargeisa", "Shanghai"
         );

   [Fact]
   public void NextReturnsStableCityListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCitySource()!,
         new string?[] {
         "Lima", "Lahore", "Rio de Janeiro", "Monaco", "Ulaanbaatar", "Basseterre", "Basseterre", "Mexico City", "Guangzhou", "Chennai", "Taipei", "Kuala Lumpur",
            "Tripoli", "Dhaka", "Ulaanbaatar", "Beijing", "Asmara", "Vatican City", null, null
         });
}
