using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DecimalSourceTests : TestBase {
   [Fact]
   public void NextReturnsNotZero() {
      var source = new DecimalSource();
      var value = source.Next(null);
      value.ShouldNotBe(0);
   }

   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestability() {
      var source = new DecimalSource(-1000, 1000, 3);
      NextReturnsStableElementListInTermsOfTestability(source, new decimal[] { 351.887M, 628.947M, 555.026M, 861.740M, -641.550M, 309.005M, -801.130M, -478.745M, -543.466M, 638.454M });
   }

   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDecimalSource(-1000, 1000, 3);
      NextReturnsStableElementListInTermsOfTestability(source, new decimal?[] { 351.887M, 628.947M, 555.026M, 861.740M, -641.550M, 309.005M, -801.130M, -478.745M, -543.466M, 638.454M });
   }
}
