using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DoubleSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableDoubleListInTermsOfTestability() {
      var source = new DoubleSource(100, 10000, 4);
      var value = source.Next(null);
      value.ShouldNotBe(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      var expectedValues = new double[] { 9315.613D, 1874.3269D, 6579.5728D, 1084.4057D, 2680.2109D, 2359.8418D, 8210.346D, 9181.7256D, 5000.4863D, 6898.7527D };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableDoubleListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDoubleSource(100, 10000, 4);
      var value = source.Next(null);
      value.ShouldNotBe(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      var expectedValues = new double?[] { 9315.613D, 1874.3269D, 6579.5728D, 1084.4057D, 2680.2109D, 2359.8418D, 8210.346D, 9181.7256D, 5000.4863D, 6898.7527D, 1927.6558D, 307.7781D, 9872.8269D, 8948.2294D, 8812.8767D, 1588.4502D, 4050.1859D, 4272.4747D, 2646.7468D, 8460.8539D };

      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextOverTheWholeRangeOfTheTypeStaysFinite() {
      // the default range used to compute max - min, which is infinity for double
      var values = Enumerable.Range(0, 200).Select(_ => new DoubleSource().Next(null)).ToList();

      values.ShouldAllBe(x => double.IsFinite(x));
      values.ShouldAllBe(x => x >= double.MinValue && x <= double.MaxValue);
   }

   [Fact]
   public void NextOverTheWholeRangeOfTheTypeVaries() {
      var source = new DoubleSource();
      var values = Enumerable.Range(0, 200).Select(_ => source.Next(null)).ToList();

      values.Distinct().Count().ShouldBe(200);
      values.ShouldContain(x => x > 0);
      values.ShouldContain(x => x < 0);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBelowMin() {
      var source = new DoubleSource(10, 5);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }
}