using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class RandomNumberSourceTests {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new RandomNumberSource(); // same as IntegerSource, exists just for compatibility reasons
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      List<int> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { -1809184581, -2056928004, -830599876, 645581134, -1620759081, 1813626657, 1895040361, -1868928598, -998188007, -2043547633 });

      List<int> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      List<int> randomGenerated2 = new();
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
