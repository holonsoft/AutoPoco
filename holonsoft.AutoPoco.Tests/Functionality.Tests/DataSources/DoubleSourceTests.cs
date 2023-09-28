using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DoubleSourceTests {
   [Fact]
   public void NextReturnsStableDoubleListInTermsOfTestability() {
      var source = new DoubleSource(100, 10000, 4);
      var value = source.Next(null);
      value.Should().NotBe(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      List<double> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { 9320.4579, 8440.4255, 9606.4981, 9582.5342, 2022.8219, 3929.1042, 3009.8279, 3076.1592, 2897.0263, 7571.7751 });

      List<double> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      List<double> randomGenerated2 = new();
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