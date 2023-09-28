using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CompanySourceTest() {
   [Fact]
   public void NextReturnsStableCompanyListInTermsOfTestability() {
      var source = new CompanySource();

      List<string> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { "Sto Plains Holdings", "Ankh-Sto Associates", "Gringotts", "Minuteman Cafe", "Niagular", "Moes Tavern", "Moes Tavern", "Praxis Corporation", "Kumatsu Motors", "Mammoth Pictures" });

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
