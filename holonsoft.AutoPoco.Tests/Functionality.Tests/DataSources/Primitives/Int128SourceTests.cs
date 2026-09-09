using Shouldly;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class Int128SourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new Int128Source();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new Int128[] { Int128.Parse("60987084906937984112046819220676830573"), Int128.Parse("33835948027648810824318586988430284115"), Int128.Parse("77675183650409931858877803340199100256"), Int128.Parse("142015491253806660708691336154857305052"), Int128.Parse("63545246249042533839977056743032445794"), Int128.Parse("7141739557454812798578630150314014694"), Int128.Parse("133989770755707609774151602249422214161"), Int128.Parse("51160944211386362012696806524424246320"), Int128.Parse("143416118351510298964194540107973806313"), Int128.Parse("117237721780817223276236274990635170371") };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableInt128Source();
      var expectedValues = new Int128?[] { Int128.Parse("59870440665555846531897554548306763247"), Int128.Parse("94432775096124116087659504134120137108"), Int128.Parse("60987084906937984112046819220676830573"), Int128.Parse("33835948027648810824318586988430284115"), Int128.Parse("77675183650409931858877803340199100256"), Int128.Parse("142015491253806660708691336154857305052"), Int128.Parse("63545246249042533839977056743032445794"), Int128.Parse("7141739557454812798578630150314014694"), Int128.Parse("133989770755707609774151602249422214161"), Int128.Parse("51160944211386362012696806524424246320") };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}