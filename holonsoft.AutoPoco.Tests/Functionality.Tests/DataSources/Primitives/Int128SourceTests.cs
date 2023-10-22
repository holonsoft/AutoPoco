using FluentAssertions;
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

      value2.Should().NotBe(value1);

      var expectedValues = new Int128[] {
         Int128.Parse("143690171173498817360163827402810620233"),
         Int128.Parse("79084523709590909672801088012826681208"),
         Int128.Parse("74823831097960774313025574080029609872"),
         Int128.Parse("159407834237347846052185826375228211524"),
         Int128.Parse("52223885703587809960109664647061407365"),
         Int128.Parse("139788082672604822438111731211146017605"),
         Int128.Parse("126225536855414366150553758439335117946"),
         Int128.Parse("28314137635927039146777420747176845036"),
         Int128.Parse("59657195644755483058324710281895298770"),
         Int128.Parse("94647823324471480647605927980975812549")
      };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableInt128Source();
      var expectedValues = new Int128?[] {
         Int128.Parse("53588579211354529504051107809525091762"),
         null,
         Int128.Parse("65806877381257608849394510731932166095"),
         Int128.Parse("143690171173498817360163827402810620233"),
         Int128.Parse("79084523709590909672801088012826681208"),
         Int128.Parse("74823831097960774313025574080029609872"),
         Int128.Parse("159407834237347846052185826375228211524"),
         Int128.Parse("52223885703587809960109664647061407365"),
         Int128.Parse("139788082672604822438111731211146017605"),
         Int128.Parse("126225536855414366150553758439335117946")
      };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}