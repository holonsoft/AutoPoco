using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CountrySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountrySource(),
         "Croatia", "Brazil", "Gambia", "Uganda", "Sri Lanka", "Uzbekistan", "Uruguay", "Comoros", "India", "Fiji"
      );

   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCountrySource()!,
         "Croatia", "Brazil", "Gambia", "Uganda", "Sri Lanka", "Uzbekistan", "Uruguay", "Comoros", "India", "Fiji",
         "Finland", "Eswatini", "Saint Vincent and the Grenadines", "Austria", "Sri Lanka", "Algeria", "Tanzania",
         "Ukraine", null, null, "Syria", "Albania"
      );

   [Fact]
   public void NextReturnsStableCountryAbbreviationListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountrySource(true),
         "HRV", "BRA", "GMB", "UGA", "LKA", "UZB", "URY", "COM", "IND", "FJI"
      );

   [Fact]
   public void NextReturnsStableCountryAbbreviationListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCountrySource(true)!,
         "HRV", "BRA", "GMB", "UGA", "LKA", "UZB", "URY", "COM", "IND", "FJI",
         "FIN", "SWZ", "VCT", "AUT", "LKA", "DZA", "TZA",
         "UKR", null, null, "SYR", "ALB"
      );

}
