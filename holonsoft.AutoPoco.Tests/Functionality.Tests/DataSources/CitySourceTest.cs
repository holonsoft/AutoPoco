using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CitySourceTest() : FixedStringArrayDataSourceTest<CitySource> {
   [Fact]
   public void NextReturnsStableCityListInTermsOfTestability()
      => PerformTest("Farmington", "Waycross", "Isle of Palms", "Rome", "Guthrie", "Stockholm", "Stockholm", "Cleveland", "Murray", "Fremont");
}
