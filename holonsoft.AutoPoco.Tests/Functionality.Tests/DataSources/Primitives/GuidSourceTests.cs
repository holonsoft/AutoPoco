using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class GuidSourceTests : TestBase {
   [Fact]
   public void NextReturnsGuid() {
      var source = new GuidSource();
      var value = source.Next(null);
      value.ShouldNotBe(Guid.Empty);
   }

   [Fact]
   public void NextWithOneSourceReturnsDifferentGuids() {
      var source = new GuidSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);
      value1.ShouldNotBe(value2);
   }

   [Fact]
   public void NextReturnsStableGuidListInTermsOfTestability() {
      var source = new GuidSource();
      var expectedValues = new Guid[] { new Guid("4f822edc-a0a0-ad0a-ef5d-88ce515881d0"), new Guid("1e263e43-1747-c70b-94e1-9d89137f4dee"), new Guid("1a887873-af70-2de1-6d65-da9478768da7"), new Guid("710ef3b1-9109-1974-53d5-92c39179b842"), new Guid("00b301f8-b23b-3a6f-60ff-3f6121dab8d1"), new Guid("c62ef017-304f-ead7-dc73-b7bda73ab87e"), new Guid("a7fbfcc2-5e87-afce-625b-b50b3eb6422f"), new Guid("1d45a92c-72f2-055f-e637-0451c923b6fc"), new Guid("9449f97a-7dbd-e4cd-1114-ee5dbe7b4de1"), new Guid("336dd8a8-3d4c-267d-3054-6b36cc6e2566") };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableGuidListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableGuidSource();
      var expectedValues = new Guid?[] { new Guid("4f822edc-a0a0-ad0a-ef5d-88ce515881d0"), new Guid("1e263e43-1747-c70b-94e1-9d89137f4dee"), new Guid("1a887873-af70-2de1-6d65-da9478768da7"), new Guid("710ef3b1-9109-1974-53d5-92c39179b842"), new Guid("00b301f8-b23b-3a6f-60ff-3f6121dab8d1"), new Guid("c62ef017-304f-ead7-dc73-b7bda73ab87e"), new Guid("a7fbfcc2-5e87-afce-625b-b50b3eb6422f"), new Guid("1d45a92c-72f2-055f-e637-0451c923b6fc"), new Guid("9449f97a-7dbd-e4cd-1114-ee5dbe7b4de1"), new Guid("336dd8a8-3d4c-267d-3054-6b36cc6e2566") };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}