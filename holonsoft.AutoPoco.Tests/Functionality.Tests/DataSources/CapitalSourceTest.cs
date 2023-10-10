using Xunit;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CapitalSourceTest() : FixedStringArrayDataSourceTest<CapitalSource> {
   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestability()
      => PerformTest("Ankara", "Stockholm", "Kuwait City", "Algiers", "Amman", "Nouakchott", "Nouakchott", "Athens", "Damascus", "Nairobi");
}