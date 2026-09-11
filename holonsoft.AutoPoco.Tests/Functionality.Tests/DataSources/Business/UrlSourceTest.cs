using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class UrlSourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new UrlSource(), new string[] { "http://www.varh.ua", "http://www.pislmnk.iq", "http://www.stmbxgpy.pe", "http://www.kqva.in", "http://www.xyxismavc.om", "http://www.jzbwg.tr", "http://www.pkd.nz", "http://www.ikcoogs.by", "http://www.aozzhwbm.ee", "http://www.hsck.pl", "http://www.pdlfxj.ua", "http://www.puckcn.ke", "http://www.nixcw.vn", "http://www.wflid.me", "http://www.sobnyuv.mv", "http://www.esmovka.ru", "http://www.zqvpyte.si", "http://www.bdfztuua.al", "http://www.hapccijl.pk", "http://www.ggrfehefpp.mx" });

   [Fact]
   public void NextReturnsStableUrlListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableUrlSource()!, new string[] { "http://www.varh.ua", "http://www.pislmnk.iq", "http://www.stmbxgpy.pe", "http://www.kqva.in", "http://www.xyxismavc.om", "http://www.jzbwg.tr", "http://www.pkd.nz", "http://www.ikcoogs.by", "http://www.aozzhwbm.ee", "http://www.hsck.pl", "http://www.pdlfxj.ua", "http://www.puckcn.ke", "http://www.nixcw.vn", "http://www.wflid.me", "http://www.sobnyuv.mv", "http://www.esmovka.ru", "http://www.zqvpyte.si", "http://www.bdfztuua.al", "http://www.hapccijl.pk", "http://www.ggrfehefpp.mx", "http://www.mdztzkew.pe", "http://www.fymd.ir" });

   [Fact]
   public void TheHostFollowsTheSessionSeedAndNotOnlyTheTopLevelDomain() {
      // the nested string source used to keep the default seed, so every session produced the same hosts
      static string[] Hosts(int seed) {
         var source = new UrlSource();
         ((ISessionSeedable) source).ApplySessionSeed(seed);
         return Enumerable.Range(0, 5)
            .Select(_ => source.Next(null).Split('.')[1])
            .ToArray();
      }

      Hosts(1337).ShouldNotBe(Hosts(4711));
   }

   [Fact]
   public void NextUsesTheLastTopLevelDomainAsWell() {
      var source = new UrlSource();
      var tlds = Enumerable.Range(0, 5000).Select(_ => source.Next(null).Split('.')[^1]).ToHashSet();

      tlds.ShouldContain("ye");
   }
}

