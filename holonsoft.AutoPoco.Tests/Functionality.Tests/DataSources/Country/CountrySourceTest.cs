using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CountrySourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountrySource(),
         // TODO: this list is depending on OS, WIN 11 has this result: 
         "Cook Islands", "British Indian Ocean Territory", "France", "U.S. Outlying Islands", "St. Pierre & Miquelon", 
         "Uruguay", "United States", "Colombia", "Guyana", "Europe"
         );

   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestabilityAndListCanContainNull()
         => NextReturnsStableElementListInTermsOfTestability(
            new NullableCountrySource()!,
            "Cook Islands", "British Indian Ocean Territory", "France", "U.S. Outlying Islands", "St. Pierre & Miquelon", 
            "Uruguay", "United States", "Colombia", "Guyana", "Europe", "Faroe Islands", "Eswatini", "Rwanda", "Australia", "St. Pierre & Miquelon",
            "Algeria", "Tanzania", "U.S. Virgin Islands", null, null
            );
}
