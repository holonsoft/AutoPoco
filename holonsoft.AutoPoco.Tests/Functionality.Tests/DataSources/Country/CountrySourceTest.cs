using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CountrySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountrySource(), new string[] { "Guatemala", "Samoa", "Monaco", "Marshall Islands", "Tunisia", "Jamaica", "Lebanon", "Brazil", "Zimbabwe", "Liberia" });

   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCountrySource()!, new string[] { "Guatemala", "Samoa", "Monaco", "Marshall Islands", "Tunisia", "Jamaica", "Lebanon", "Brazil", "Zimbabwe", "Liberia", "Denmark", "Nauru", "Belize", "Switzerland", "East Timor", "New Zealand", "Guyana", "Guatemala", "Mauritania", "Burundi", "Samoa", "Spain" });

   [Fact]
   public void NextReturnsStableCountryAbbreviationListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountrySource(true), new string[] { "GTM", "WSM", "MCO", "MHL", "TUN", "JAM", "LBN", "BRA", "ZWE", "LBR" });

   [Fact]
   public void NextReturnsStableCountryAbbreviationListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCountrySource(true)!, new string[] { "GTM", "WSM", "MCO", "MHL", "TUN", "JAM", "LBN", "BRA", "ZWE", "LBR", "DNK", "NRU", "BLZ", "CHE", "TLS", "NZL", "GUY", "GTM", "MRT", "BDI", "WSM", "ESP" });

}
