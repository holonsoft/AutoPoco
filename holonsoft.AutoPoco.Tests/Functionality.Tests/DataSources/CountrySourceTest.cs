using Xunit;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CountrySourceTest() : FixedStringArrayDataSourceTest<CountrySource> {
   [Fact]
   public void NextReturnsStableCountryListInTermsOfTestability()
      => PerformTest("Cook Islands", "British Indian Ocean Territory", "France", "U.S. Outlying Islands", "St. Pierre & Miquelon", "Uruguay", "United States", "Colombia", "Guyana", "Europe");
}
