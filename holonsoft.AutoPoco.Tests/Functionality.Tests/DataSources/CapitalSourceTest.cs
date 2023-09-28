using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CapitalSourceTest() : FixedStringArrayDataSourceTest<CapitalSource> {
   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestability()
      => PerformTest("Ankara", "Stockholm", "Kuwait City", "Algiers", "Amman", "Nouakchott", "Nouakchott", "Athens", "Damascus", "Nairobi");
}