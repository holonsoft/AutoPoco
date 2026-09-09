using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class UrlSourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new UrlSource(), new string[] { "http://www.pdutnrt.ua", "http://www.axc.iq", "http://www.mgriq.pe", "http://www.jgdowudn.in", "http://www.siowk.om", "http://www.faqdogfvk.tr", "http://www.wqj.nz", "http://www.scfmvsendy.by", "http://www.bmey.ee", "http://www.ksxgwj.pl", "http://www.nxud.ua", "http://www.qgoxobql.ke", "http://www.aqfbywdw.vn", "http://www.vxwhakt.me", "http://www.ebdiqmt.mv", "http://www.oqmorhrsed.ru", "http://www.idqhtrremq.si", "http://www.qswnqa.al", "http://www.aop.pk", "http://www.hbvimbnfj.mx" });

   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableUrlSource()!, new string[] { "http://www.pdutnrt.ua", "http://www.axc.iq", "http://www.mgriq.pe", "http://www.jgdowudn.in", "http://www.siowk.om", "http://www.faqdogfvk.tr", "http://www.wqj.nz", "http://www.scfmvsendy.by", "http://www.bmey.ee", "http://www.ksxgwj.pl", "http://www.nxud.ua", "http://www.qgoxobql.ke", "http://www.aqfbywdw.vn", "http://www.vxwhakt.me", "http://www.ebdiqmt.mv", "http://www.oqmorhrsed.ru", "http://www.idqhtrremq.si", "http://www.qswnqa.al", "http://www.aop.pk", "http://www.hbvimbnfj.mx", "http://www.elp.pe", "http://www.hjxmtllbs.ir" });
}

