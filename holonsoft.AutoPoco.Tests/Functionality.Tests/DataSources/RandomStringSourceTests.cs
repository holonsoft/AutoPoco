using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class RandomStringSourceTests {
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

      List<string> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { "GSwqx", "LWRRQlC", "AtwsA[s", "Gk[ZGqr", "ew^]i_`", "GvJEb", "MMXmwlR", "suUJM", "W_rU`", "gUOaI\\" })
         .And.ContainItemsAssignableTo<string>();

      List<string> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And.ContainItemsAssignableTo<string>();

      List<string> randomGenerated2 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And.ContainItemsAssignableTo<string>();

      randomGenerated1.Should().NotBeEquivalentTo(generated);
      randomGenerated2.Should().NotBeEquivalentTo(generated);
      randomGenerated1.Should().NotBeEquivalentTo(randomGenerated2);
   }
}