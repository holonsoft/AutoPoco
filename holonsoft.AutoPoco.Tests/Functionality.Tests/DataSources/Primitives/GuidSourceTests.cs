using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class GuidSourceTests : TestBase {
   [Fact]
   public void NextReturnsGuid() {
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
   public void NextReturnsStableGuidListInTermsOfTestability() {
      var source = new GuidSource();
      var expectedValues = new Guid[] {
         new("c7f40068-5e43-aa02-c27c-4fd927fc2227"),
         new("b254896a-12e5-1eef-9af7-227ef036e328"),
         new("c1d474a5-ba17-69e3-c756-e60d4fa4da45"),
         new("baf3bb5a-59f6-5524-10d6-2d4c3c84b98b"),
         new("b3332698-6127-b00f-a6eb-ea8f2ce2cef6"),
         new("3423dd1d-2943-4c32-4f7f-b1418d962c43"),
         new("29755a9c-2670-f91d-ac43-130a6f95282d"),
         new("5f2f3607-8461-6f48-f4e7-63a8afce220f"),
         new("d2f2a7bf-a56d-f685-bf29-88ff1527dcba"),
         new("56398356-cd67-18e1-1206-3489c3754ade")
      };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableGuidListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableGuidSource();
      var expectedValues = new Guid?[] {
         new("c7f40068-5e43-aa02-c27c-4fd927fc2227"),
         null,
         new("b254896a-12e5-1eef-9af7-227ef036e328"),
         new("c1d474a5-ba17-69e3-c756-e60d4fa4da45"),
         new("baf3bb5a-59f6-5524-10d6-2d4c3c84b98b"),
         new("b3332698-6127-b00f-a6eb-ea8f2ce2cef6"),
         new("3423dd1d-2943-4c32-4f7f-b1418d962c43"),
         new("29755a9c-2670-f91d-ac43-130a6f95282d"),
         new("5f2f3607-8461-6f48-f4e7-63a8afce220f"),
         new("d2f2a7bf-a56d-f685-bf29-88ff1527dcba")
      };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}