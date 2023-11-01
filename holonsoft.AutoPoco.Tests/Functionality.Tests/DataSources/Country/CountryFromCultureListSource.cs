using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CountryFromCultureListSourceTest() : TestBase {
   [Fact(Skip = "Depends on OS version, underlying culture list is not stable in terms of repeatable test on different machines")]
   public void NextReturns_NotA_StableCountryListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CountryFromCultureListSource(),
         "Cook Islands", "British Indian Ocean Territory", "France", "U.S. Outlying Islands", "St. Pierre & Miquelon",
         "Uruguay", "United States", "Colombia", "Guyana", "Europe"
      );

   [Fact(Skip = "Depends on OS version, underlying culture list is not stable in terms of repeatable test on different machines")]
   public void NextReturns_NotA_StableCountryListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCountryFromCultureListSource()!,
         "Cook Islands", "British Indian Ocean Territory", "France", "U.S. Outlying Islands", "St. Pierre & Miquelon",
         "Uruguay", "United States", "Colombia", "Guyana", "Europe", "Faroe Islands", "Eswatini", "Rwanda", "Australia", "St. Pierre & Miquelon",
         "Algeria", "Tanzania", "U.S. Virgin Islands", null, null
      );
}