using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class LongSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new LongSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      var expectedValues = new long[] { -2088576094730105571L, -4011664040411344994L, 6355547538257366377L, 8014153468887495314L, -649010795262790965L, -7229896734490018064L, -1110956013845024003L, 6398855854917328672L, 8059659982279395047L, 3798038521325847177L };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableLongSource();
      var expectedValues = new long?[] { -3413286636718563731L, null, 8490162135550892900L, -2088576094730105571L, -4011664040411344994L, 6355547538257366377L, 8014153468887495314L, -649010795262790965L, -7229896734490018064L, -1110956013845024003L };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}