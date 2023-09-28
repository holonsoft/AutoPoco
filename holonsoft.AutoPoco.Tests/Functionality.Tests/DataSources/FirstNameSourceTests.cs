using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class FirstNameSourceTests : FixedStringArrayDataSourceTest<FirstNameSource> {
   [Fact]
   public void NextReturnsStableFirstNameListInTermsOfTestability()
      => PerformTest("George", "Daniel", "Mohammed", "Evelin", "Luna", "David", "David", "Samuel", "Ruby", "Lewis");
}