using System.Numerics;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class NumberSourceTests : TestBase {
   private static List<T> Draw<T>(NumberSource<T> source, int count)
      where T : struct, INumber<T>, IMinMaxValue<T>
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   private static void AssertStaysInRange<T>(T min, T max, int count = 2000)
      where T : struct, INumber<T>, IMinMaxValue<T> {
      var values = Draw(new NumberSource<T>(min, max), count);

      values.Count.ShouldBe(count);
      // ShouldAllBe takes an expression tree, generic math operators are not allowed in there
      values.Where(x => x < min || x > max).ShouldBeEmpty($"{typeof(T).Name} [{min}, {max}]");
   }

   private static void AssertReachesBothEnds<T>(T min, T max, int count = 500)
      where T : struct, INumber<T>, IMinMaxValue<T> {
      var values = Draw(new NumberSource<T>(min, max), count);

      values.ShouldContain(min, $"{typeof(T).Name} never produced its minimum {min}");
      values.ShouldContain(max, $"{typeof(T).Name} never produced its maximum {max}");
   }

   [Fact]
   public void NextStaysInRangeForEveryBuiltInIntegerType() {
      AssertStaysInRange<sbyte>(-100, 100);
      AssertStaysInRange<byte>(10, 20);
      AssertStaysInRange<short>(-1000, 1000);
      AssertStaysInRange<ushort>(100, 200);
      AssertStaysInRange(-1_000_000, 1_000_000);
      AssertStaysInRange<uint>(1, 3_000_000_000);
      AssertStaysInRange(-5_000_000_000L, 5_000_000_000L);
      AssertStaysInRange<ulong>(1, 10_000_000_000_000_000_000UL);
      AssertStaysInRange<nint>(-1000, 1000);
      AssertStaysInRange<nuint>(1, 1000);
      AssertStaysInRange((Int128) long.MinValue * 4, (Int128) long.MaxValue * 4);
      AssertStaysInRange<UInt128>(1, (UInt128) ulong.MaxValue * 4);
      AssertStaysInRange<char>('a', 'z');
   }

   [Fact]
   public void NextStaysInRangeForEveryBuiltInFloatingPointType() {
      AssertStaysInRange(-1.5f, 2.5f);
      AssertStaysInRange(-1.5, 2.5);
      AssertStaysInRange((Half) (-1.5), (Half) 2.5);
      AssertStaysInRange(-1.5m, 2.5m);
   }

   [Fact]
   public void NextCoversTheWholeRangeOfTheTypeByDefault() {
      AssertStaysInRange(sbyte.MinValue, sbyte.MaxValue);
      AssertStaysInRange(int.MinValue, int.MaxValue);
      AssertStaysInRange(ulong.MinValue, ulong.MaxValue);
      AssertStaysInRange(Int128.MinValue, Int128.MaxValue);
      AssertStaysInRange(UInt128.MinValue, UInt128.MaxValue);
      AssertStaysInRange(float.MinValue, float.MaxValue);
      AssertStaysInRange(double.MinValue, double.MaxValue);
      AssertStaysInRange(Half.MinValue, Half.MaxValue);
      AssertStaysInRange(decimal.MinValue, decimal.MaxValue);
   }

   [Fact]
   public void NextOverTheWholeDoubleRangeProducesFiniteValuesOfBothSigns() {
      var values = Draw(new NumberSource<double>(), 1000);

      values.ShouldAllBe(x => double.IsFinite(x));
      values.ShouldContain(x => x < 0);
      values.ShouldContain(x => x > 0);
   }

   [Fact]
   public void NextOverTheWholeIntegerRangeUsesTheWholeWidth() {
      Draw(new NumberSource<long>(), 1000).ShouldContain(x => x > int.MaxValue || x < int.MinValue);
      Draw(new NumberSource<ulong>(), 1000).ShouldContain(x => x > long.MaxValue);
      Draw(new NumberSource<Int128>(), 1000).ShouldContain(x => x > long.MaxValue || x < long.MinValue);
      Draw(new NumberSource<UInt128>(), 1000).ShouldContain(x => x > ulong.MaxValue);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortIntegerRange() {
      AssertReachesBothEnds<byte>(250, 255);
      AssertReachesBothEnds<sbyte>(-128, -125);
      AssertReachesBothEnds(1, 6);
      AssertReachesBothEnds(int.MaxValue - 3, int.MaxValue);
      AssertReachesBothEnds(long.MinValue, long.MinValue + 3);
      AssertReachesBothEnds(UInt128.MaxValue - 3, UInt128.MaxValue);
      AssertReachesBothEnds(Int128.MinValue, Int128.MinValue + 3);
   }

   [Fact]
   public void NextReachesEveryValueOfAByte() {
      var values = Draw(new NumberSource<byte>(), 20_000);

      values.Distinct().Count().ShouldBe(256);
   }

   [Fact]
   public void NextIsRoughlyUniformOverASmallRange() {
      var histogram = Draw(new NumberSource<int>(1, 10), 10_000)
         .GroupBy(x => x)
         .ToDictionary(g => g.Key, g => g.Count());

      histogram.Keys.OrderBy(x => x).ShouldBe(Enumerable.Range(1, 10));
      histogram.Values.ShouldAllBe(count => count > 850 && count < 1150);
   }

   [Fact]
   public void NextIsRoughlyUniformOverARangeWiderThanLong() {
      // width above long.MaxValue takes the 128 bit path with rejection sampling
      var min = (UInt128) 1 << 70;
      var max = min + ((UInt128) 1 << 71);
      var quarter = (max - min) / 4;
      var values = Draw(new NumberSource<UInt128>(min, max), 4000);

      values.ShouldAllBe(x => x >= min && x <= max);
      values.Count(x => x < min + quarter).ShouldBeInRange(800, 1200);
      values.Count(x => x >= max - quarter).ShouldBeInRange(800, 1200);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      Draw(new NumberSource<int>(42, 42), 20).ShouldAllBe(x => x == 42);
      Draw(new NumberSource<decimal>(4.2m, 4.2m), 20).ShouldAllBe(x => x == 4.2m);
      Draw(new NumberSource<UInt128>(UInt128.MaxValue, UInt128.MaxValue), 20).ShouldAllBe(x => x == UInt128.MaxValue);
   }

   [Fact]
   public void NextOnADecimalRangeProducesFractions() {
      var values = Draw(new NumberSource<decimal>(0m, 1m), 100);

      values.ShouldAllBe(x => x >= 0m && x <= 1m);
      values.ShouldContain(x => x != decimal.Truncate(x));
   }

   [Fact]
   public void ConstructorThrowsWhenMaxIsBelowMin() {
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<int>(10, 1));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<double>(0.2, 0.1));
      Should.Throw<ArgumentOutOfRangeException>(() => new NullableNumberSource<byte>(10, 1));
   }

   [Fact]
   public void ConstructorThrowsOnNaNOrInfinity() {
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<double>(double.NaN, 1));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<double>(0, double.NaN));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<double>(double.NegativeInfinity, 0));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<float>(0, float.PositiveInfinity));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<Half>(Half.NaN, Half.One));
   }

   [Fact]
   public void SetMinMaxThrowsWhenMaxIsBelowMinAndKeepsTheOldRange() {
      var source = new NumberSource<int>(1, 10);

      Should.Throw<ArgumentOutOfRangeException>(() => source.SetMinMax(5, 4));

      source.Min.ShouldBe(1);
      source.Max.ShouldBe(10);
   }

   [Fact]
   public void SetMinMaxSetMinAndSetMaxChangeTheRange() {
      var source = new NumberSource<int>();

      source.SetMinMax(100, 200);
      Draw(source, 200).ShouldAllBe(x => x >= 100 && x <= 200);

      source.SetMin(150);
      Draw(source, 200).ShouldAllBe(x => x >= 150 && x <= 200);

      source.SetMax(160);
      Draw(source, 200).ShouldAllBe(x => x >= 150 && x <= 160);

      Should.Throw<ArgumentOutOfRangeException>(() => source.SetMin(161));
      Should.Throw<ArgumentOutOfRangeException>(() => source.SetMax(149));
   }

   [Fact]
   public void DefaultConstructorUsesTheWholeRangeOfTheType() {
      new NumberSource<short>().Min.ShouldBe(short.MinValue);
      new NumberSource<short>().Max.ShouldBe(short.MaxValue);
      new NumberSource<decimal>().Min.ShouldBe(decimal.MinValue);
      new NumberSource<decimal>().Max.ShouldBe(decimal.MaxValue);
      new NullableNumberSource<Half>().Min.ShouldBe(Half.MinValue);
      new NullableNumberSource<Half>().Max.ShouldBe(Half.MaxValue);
   }

   [Fact]
   public void NullableSourceHonorsAnExplicitNullCreationThreshold() {
      var never = new NullableNumberSource<int>(1, 10, 0);
      var always = new NullableNumberSource<int>(1, 10, 100);
      var sometimes = new NullableNumberSource<int>(1, 10, 50);
      var alwaysWholeRange = new NullableNumberSource<long>(100);

      Enumerable.Range(0, 200).Select(_ => never.Next(null)).ShouldAllBe(x => x != null && x >= 1 && x <= 10);
      Enumerable.Range(0, 200).Select(_ => always.Next(null)).ShouldAllBe(x => x == null);
      Enumerable.Range(0, 200).Select(_ => alwaysWholeRange.Next(null)).ShouldAllBe(x => x == null);

      var nullCount = Enumerable.Range(0, 1000).Select(_ => sometimes.Next(null)).Count(x => x == null);
      nullCount.ShouldBeInRange(400, 600);
   }

   [Fact]
   public void NextReturnsStableNumberListInTermsOfTestability() {
      var source = new NumberSource<int>(1, 100);
      NextReturnsStableElementListInTermsOfTestability(source, 41, 50, 37, 60, 14, 57, 91, 40, 22, 52);
   }

   [Fact]
   public void NextReturnsStableDecimalListInTermsOfTestability() {
      var source = new NumberSource<decimal>(0m, 10m);
      NextReturnsStableElementListInTermsOfTestability(source, 2.088883408386670m, 1.18017077500940m, 3.14965957922380m, 9.313593841769540m, 8.424672204267550m);
   }

   [Fact]
   public void NextReturnsStableNumberListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableNumberSource<int>(1, 100);
      NextReturnsStableElementListInTermsOfTestability(source, new int?[] { 41, null, 50, 37, 60, 14, 57, 91, 40, 22 });
   }
}
