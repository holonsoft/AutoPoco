using Shouldly;
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

         value.Length.ShouldBeGreaterThanOrEqualTo(min);
         value.Length.ShouldBeLessThanOrEqualTo(max);
      }
   }

   [Fact]
   public void NextReturnsStableStringsInTermsOfTestability() {
      var source = new RandomStringSource(5, 7, 'A', (char) 123);
      NextReturnsStableElementListInTermsOfTestability(source, new string[] { "pDUtn", "TyaX]C", "mg{Riqj", "Do\\wUd[", "ns[SIow", "_FaQD^o", "FVKa^WQ", "`ScFmV", "End`yZB", "E]yDK" });
   }

   [Fact]
   public void NextReturnsStableStringsInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableRandomStringSource(5, 7, 'A', (char) 123);
      NextReturnsStableElementListInTermsOfTestability<string?>(source!, new string?[] { "pDUtn", "TyaX]C", "mg{Riqj", "Do\\wUd[", "ns[SIow", "_FaQD^o", "FVKa^WQ", "`ScFmV", "End`yZB", "E]yDK" });
   }

   [Fact]
   public void NextUsesTheLastAllowedCharacterAsWell() {
      var source = new RandomStringSource(20, 20, 'a', 'c');
      var used = string.Concat(Enumerable.Range(0, 50).Select(_ => source.Next(null)));

      used.ShouldContain('a');
      used.ShouldContain('b');
      used.ShouldContain('c');
   }

   [Fact]
   public void NextUsesTheLastCharacterOfAnExplicitCharacterSet() {
      var source = new RandomStringSource(20, 20, new[] { 'x', 'y', 'z' });
      var used = string.Concat(Enumerable.Range(0, 50).Select(_ => source.Next(null)));

      used.ShouldContain('z');
   }
}
