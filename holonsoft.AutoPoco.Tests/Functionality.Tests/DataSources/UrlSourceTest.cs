using Xunit;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class UrlSourceTest() : FixedStringArrayDataSourceTestBase<UrlSource> {
   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestability()
      => PerformTest("http://www.chxv.de", "http://www.xejhhhsbva.com", "http://www.xvalvxcslk.nl", "http://www.vvt.org", "http://www.xmmrnnhcco.uk", "http://www.ebouffktxs.org", "http://www.hvwie.org", "http://www.ejnv.de", "http://www.nmqig.nl", "http://www.dmiptch.nl");
}
