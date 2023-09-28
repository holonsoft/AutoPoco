using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class GuidSourceTests {
   [Fact]
   public void NextReturnsAGuid() {
      var source = new GuidSource();
      var value = source.Next(null);
      value.Should().NotBe(Guid.Empty);
   }

   [Fact]
   public void NextWithOneSourceReturnsDifferentGuids() {
      var source = new GuidSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);
      value1.Should().NotBe(value2);
   }

   [Fact]
   public void RandomSeedGuaranteesDifferentGuids() {
      var source = new GuidSource();

      List<string> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null).ToString());

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { "c7f40068-5e43-aa02-c27c-4fd927fc2227", "b254896a-12e5-1eef-9af7-227ef036e328", "c1d474a5-ba17-69e3-c756-e60d4fa4da45", "baf3bb5a-59f6-5524-10d6-2d4c3c84b98b", "b3332698-6127-b00f-a6eb-ea8f2ce2cef6", "3423dd1d-2943-4c32-4f7f-b1418d962c43", "29755a9c-2670-f91d-ac43-130a6f95282d", "5f2f3607-8461-6f48-f4e7-63a8afce220f", "d2f2a7bf-a56d-f685-bf29-88ff1527dcba", "56398356-cd67-18e1-1206-3489c3754ade" });

      List<string> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null).ToString());

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And.ContainItemsAssignableTo<string>();

      List<string> randomGenerated2 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null).ToString());

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