using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class IntegerSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new IntegerSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new int[] { -1641660861, 161341844, -1702332301, 349857133, -250678351, 1133696339, -2135752200, -515899552, 1177481239, 1035432924 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableIntegerSource();
      var expectedValues = new int?[] { -813551908, 1317559791, -1641660861, 161341844, -1702332301, 349857133, -250678351, 1133696339, -2135752200, -515899552 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortRange() {
      var source = new IntegerSource(int.MaxValue - 2, int.MaxValue);
      var values = Draw(source, 300);

      values.ShouldAllBe(x => x >= int.MaxValue - 2 && x <= int.MaxValue);
      values.ShouldContain(int.MaxValue - 2);
      values.ShouldContain(int.MaxValue);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      var source = new IntegerSource(42, 42);

      Draw(source, 10).ShouldAllBe(x => x == 42);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBelowMin() {
      var source = new IntegerSource(10, 5);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetMinMaxChangesTheRange() {
      var source = new IntegerSource();
      source.SetMinMax(int.MinValue, int.MinValue + 2);

      Draw(source, 100).ShouldAllBe(x => x >= int.MinValue && x <= int.MinValue + 2);
   }

   private static List<int> Draw(IntegerSource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();
}
