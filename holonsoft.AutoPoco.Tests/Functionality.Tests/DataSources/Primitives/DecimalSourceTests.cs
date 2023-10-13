using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DecimalSourceTests : TestBase {
   [Fact]
   public void NextReturnsNotZero() {
      var source = new DecimalSource();
      var value = source.Next(null);
      value.Should().NotBe(0);
   }

   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestability() {
      var source = new DecimalSource(-1000, 1000, 3);
      NextReturnsStableElementListInTermsOfTestability(source, new decimal[] { -582.223M, -763.966M, -370.068M, 862.719M, 684.934M, 920.505M, 915.663M, -611.551M, -226.444M, -412.156M });
   }

   [Fact]
   public void NextReturnsStableBooleanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDecimalSource(-1000, 1000, 3);
      NextReturnsStableElementListInTermsOfTestability(source, new decimal?[] { -582.223M, null, -763.966M, -370.068M, 862.719M, 684.934M, 920.505M, 915.663M, -611.551M, -226.444M });
   }
}
