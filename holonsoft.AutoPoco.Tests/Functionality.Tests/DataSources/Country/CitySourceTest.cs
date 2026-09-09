using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CitySourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableCityListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CitySource(), new string[] { "Freetown", "London", "Abidjan", "Kingstown", "Ho Chi Minh City", "Tripoli", "Berlin", "Mexico City", "Xi'an", "Alexandria", "Monrovia", "Freetown", "Rio de Janeiro", "London", "Los Angeles", "Taipei", "Rome", "Lima", "Tianjin", "Jinan" });

   [Fact]
   public void NextReturnsStableCityListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCitySource()!, new string[] { "Freetown", "London", "Abidjan", "Kingstown", "Ho Chi Minh City", "Tripoli", "Berlin", "Mexico City", "Xi'an", "Alexandria", "Monrovia", "Freetown", "Rio de Janeiro", "London", "Los Angeles", "Taipei", "Rome", "Lima", "Tianjin", "Jinan" });
}
