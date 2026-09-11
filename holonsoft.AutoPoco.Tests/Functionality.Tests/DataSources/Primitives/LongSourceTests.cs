using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class LongSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new LongSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new long[] { -5917255539371771789, -7389121618870602831, -5012591893755526664, 7698675207198666775, 3444794701716913346, -8836217559678867156, 7263600027208644986, -6449931693835036504, -1448768682547491459, 6355469632600658758 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableLongSource();
      var expectedValues = new long?[] { 3245583091863006940, 5119210995652640323, -5917255539371771789, -7389121618870602831, -5012591893755526664, 7698675207198666775, 3444794701716913346, -8836217559678867156, 7263600027208644986, -6449931693835036504 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortRange() {
      var source = new LongSource(long.MaxValue - 2, long.MaxValue);
      var values = Draw(source, 300);

      values.ShouldAllBe(x => x >= long.MaxValue - 2 && x <= long.MaxValue);
      values.ShouldContain(long.MaxValue - 2);
      values.ShouldContain(long.MaxValue);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      var source = new LongSource(42, 42);

      Draw(source, 10).ShouldAllBe(x => x == 42);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBelowMin() {
      var source = new LongSource(10, 5);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetMinMaxChangesTheRangeAndKeepsTheFullLongPrecision() {
      var source = new LongSource();
      source.SetMinMax(long.MinValue, long.MinValue + 2);

      Draw(source, 100).ShouldAllBe(x => x >= long.MinValue && x <= long.MinValue + 2);
   }

   private static List<long> Draw(LongSource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();
}