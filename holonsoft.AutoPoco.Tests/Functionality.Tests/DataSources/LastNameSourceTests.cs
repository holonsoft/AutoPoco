using Xunit;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class LastNameSourceTests : FixedStringArrayDataSourceTest<LastNameSource> {
   [Fact]
   public void NextReturnsStableLastNamesListInTermsOfTestability()
      => PerformTest("Turner", "Martin", "Murphy", "Pierce", "Wells", "Scott", "Scott", "Baker", "Perez", "Evans");
}