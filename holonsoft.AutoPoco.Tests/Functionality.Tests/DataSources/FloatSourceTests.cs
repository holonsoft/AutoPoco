using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class FloatSourceTests {
   [Fact]
   public void NextReturnsStableFloatListInTermsOfTestability() {
      var source = new FloatSource(3);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      List<float> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { -1.2592764E+38F, 2.9356796E+38F, 2.330711E+38F, 3.132315E+38F, 3.1158412E+38F, -2.0810006E+38F, -7.7054755E+37F, -1.402494E+38F, -1.3568953E+38F, -1.4800383E+38F });

      List<float> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      List<float> randomGenerated2 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      randomGenerated1.Should().NotBeEquivalentTo(generated);
      randomGenerated2.Should().NotBeEquivalentTo(generated);
      randomGenerated1.Should().NotBeEquivalentTo(randomGenerated2);
   }
}