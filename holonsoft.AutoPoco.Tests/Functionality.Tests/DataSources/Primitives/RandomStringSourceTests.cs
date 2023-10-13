using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class RandomStringSourceTests : TestBase {
   [Theory]
   [InlineData(5, 10)]
   [InlineData(0, 10)]
   [InlineData(0, 2)]
   public void NextReturnsStringOfCorrectSize(int min, int max) {
      var source = new RandomStringSource(min, max);
      for (var x = 0; x < 10; x++) {
         var value = source.Next(null);

         value.Length.Should().BeGreaterThanOrEqualTo(min);
         value.Length.Should().BeLessThanOrEqualTo(max);
      }
   }

   [Fact]
   public void NextReturnsStableStringsInTermsOfTestability() {
      var source = new RandomStringSource(5, 7, 'A', (char) 123);
      NextReturnsStableElementListInTermsOfTestability(source, new[] {
         "GSwqx", "LWRRQlC", "AtwsA[s", "Gk[ZGqr", "ew^]i_`", "GvJEb", "MMXmwlR", "suUJM", "W_rU`", "gUOaI\\"
      });
   }

   [Fact]
   public void NextReturnsStableStringsInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableRandomStringSource(5, 7, 'A', (char) 123);
      NextReturnsStableElementListInTermsOfTestability<string?>(source!, new string?[] {
         "GSwqx", null, "LWRRQlC", "AtwsA[s", "Gk[ZGqr", "ew^]i_`", "GvJEb", "MMXmwlR", "suUJM", "W_rU`"
      });
   }
}
