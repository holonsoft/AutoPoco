using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class FloatSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableFloatListInTermsOfTestability() {
      var source = new FloatSource(3);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new float[] { 1.8886552E+38F, 2.932349E+38F, -2.1830817E+38F, 1.0514879E+38F, -2.7261045E+38F, -1.6290856E+38F, -1.8493199E+38F, 2.1725452E+38F, 2.8403095E+38F, -3.4037737E+36F };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableFloatListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableFloatSource(3);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new float?[] { 1.8886552E+38F, 2.932349E+38F, -2.1830817E+38F, 1.0514879E+38F, -2.7261045E+38F, -1.6290856E+38F, -1.8493199E+38F, 2.1725452E+38F, 2.8403095E+38F, -3.4037737E+36F, 1.270905E+38F, -2.1464215E+38F, -3.2599887E+38F, 3.3153998E+38F, 2.6797953E+38F, 2.5867485E+38F, -2.3796047E+38F, -6.8731113E+37F, -5.3450133E+37F, -1.6520902E+38F };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBelowMin() {
      var source = new FloatSource(10f, 5f);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }
}