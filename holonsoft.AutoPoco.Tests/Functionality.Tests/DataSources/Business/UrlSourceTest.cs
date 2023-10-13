using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class UrlSourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new UrlSource(),
            "http://www.chxv.se", "http://www.xejhhhsbva.fr", "http://www.xvalvxcslk.pt", "http://www.vvt.in", "http://www.xmmrnnhc.su",
            "http://www.ebouffktxs.iq", "http://www.hvwie.iq", "http://www.ejnv.nl", "http://www.nmqig.me", "http://www.dmiptch.hu",
            "http://www.jhngaiwcl.hu", "http://www.bpdni.gr", "http://www.xobun.kg", "http://www.dvy.mil", "http://www.plkqntpot.su",
            "http://www.qdmmkkbigq.org", "http://www.odyrea.mv", "http://www.gidqhe.in", "http://www.lrtthaos.mm", "http://www.qtcnjc.com"
         );

   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableUrlSource()!,
            "http://www.chxv.se", "http://www.xejhhhsbva.fr", "http://www.xvalvxcslk.pt", "http://www.vvt.in", "http://www.xmmrnnhc.su",
            "http://www.ebouffktxs.iq", "http://www.hvwie.iq", "http://www.ejnv.nl", "http://www.nmqig.me", "http://www.dmiptch.hu",
            "http://www.jhngaiwcl.hu", "http://www.bpdni.gr", "http://www.xobun.kg", "http://www.dvy.mil", "http://www.plkqntpot.su",
            "http://www.qdmmkkbigq.org", "http://www.odyrea.mv", "http://www.gidqhe.in",
            null, null,
            "http://www.lrtthaos.mm", "http://www.qtcnjc.com"
         );
}

