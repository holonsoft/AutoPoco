#if NET11_0_OR_GREATER
using System.Globalization;
using System.Numerics;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

/// <summary>
///   The IEEE 754 decimal types of .NET 11 need no source of their own, NumberSource&lt;T&gt; covers them
///   through generic math. These tests make sure it really does.
/// </summary>
public class IeeeDecimalNumberSourceTests {
   private static List<T> Draw<T>(NumberSource<T> source, int count)
      where T : struct, INumber<T>, IMinMaxValue<T>
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   private static void AssertStaysInRange<T>(T min, T max, int count = 2000)
      where T : struct, INumber<T>, IMinMaxValue<T> {
      var values = Draw(new NumberSource<T>(min, max), count);

      values.Count.ShouldBe(count);
      values.Where(x => x < min || x > max).ShouldBeEmpty($"{typeof(T).Name} [{min}, {max}]");
      values.Where(x => !T.IsFinite(x)).ShouldBeEmpty($"{typeof(T).Name} [{min}, {max}]");
   }

   private static void AssertProducesFractionsOfBothSigns<T>()
      where T : struct, INumber<T>, IMinMaxValue<T> {
      var values = Draw(new NumberSource<T>(-T.One, T.One), 1000);

      values.Count(T.IsNegative).ShouldBeInRange(400, 600, typeof(T).Name);
      values.Count(x => !T.IsInteger(x)).ShouldBeGreaterThan(990, typeof(T).Name);
   }

   /// <summary>
   ///   Pinned as invariant text, so the number of digits (the quantum) is part of the contract as well.
   /// </summary>
   private static void AssertPinned<T>(params string[] expected)
      where T : struct, INumber<T>, IMinMaxValue<T> {
      var source = new NumberSource<T>(T.Zero, T.CreateChecked(10));

      Draw(source, expected.Length).Select(x => x.ToString(null, CultureInfo.InvariantCulture)).ShouldBe(expected);
   }

   [Fact]
   public void NextStaysInRange() {
      AssertStaysInRange(Decimal32.Parse("-1.5", CultureInfo.InvariantCulture), Decimal32.Parse("2.5", CultureInfo.InvariantCulture));
      AssertStaysInRange(Decimal64.Parse("-1.5", CultureInfo.InvariantCulture), Decimal64.Parse("2.5", CultureInfo.InvariantCulture));
      AssertStaysInRange(Decimal128.Parse("-1.5", CultureInfo.InvariantCulture), Decimal128.Parse("2.5", CultureInfo.InvariantCulture));
   }

   [Fact]
   public void NextCoversTheWholeRangeOfTheTypeWithoutOverflow() {
      AssertStaysInRange(Decimal32.MinValue, Decimal32.MaxValue);
      AssertStaysInRange(Decimal64.MinValue, Decimal64.MaxValue);
      AssertStaysInRange(Decimal128.MinValue, Decimal128.MaxValue);
   }

   [Fact]
   public void NextProducesFractionsOfBothSigns() {
      AssertProducesFractionsOfBothSigns<Decimal32>();
      AssertProducesFractionsOfBothSigns<Decimal64>();
      AssertProducesFractionsOfBothSigns<Decimal128>();
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      var value = Decimal64.Parse("3.25", CultureInfo.InvariantCulture);

      Draw(new NumberSource<Decimal64>(value, value), 50).ShouldAllBe(x => x == value);
   }

   [Fact]
   public void DefaultConstructorUsesTheWholeRangeOfTheType() {
      new NumberSource<Decimal32>().Min.ShouldBe(Decimal32.MinValue);
      new NumberSource<Decimal32>().Max.ShouldBe(Decimal32.MaxValue);
      new NumberSource<Decimal64>().Min.ShouldBe(Decimal64.MinValue);
      new NumberSource<Decimal128>().Max.ShouldBe(Decimal128.MaxValue);
   }

   [Fact]
   public void ConstructorThrowsOnNaNInfinityOrAReversedRange() {
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<Decimal32>(Decimal32.NaN, Decimal32.One));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<Decimal64>(Decimal64.Zero, Decimal64.PositiveInfinity));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<Decimal128>(Decimal128.NegativeInfinity, Decimal128.Zero));
      Should.Throw<ArgumentOutOfRangeException>(() => new NumberSource<Decimal64>(Decimal64.One, Decimal64.Zero));
   }

   [Fact]
   public void NullableSourceHonorsAnExplicitNullCreationThreshold() {
      var never = new NullableNumberSource<Decimal64>(Decimal64.Zero, Decimal64.One, 0);
      var always = new NullableNumberSource<Decimal128>(Decimal128.Zero, Decimal128.One, 100);

      Enumerable.Range(0, 200).Select(_ => never.Next(null)).ShouldAllBe(x => x != null);
      Enumerable.Range(0, 200).Select(_ => always.Next(null)).ShouldAllBe(x => x == null);
   }

   [Fact]
   public void Decimal32SequenceIsPinned()
      => AssertPinned<Decimal32>("6.759434", "8.144736", "7.775130", "9.308700", "1.792249");

   [Fact]
   public void Decimal64SequenceIsPinned()
      => AssertPinned<Decimal64>("6.759434119590045", "8.144736480449252", "7.775129841449137", "9.308700010966429", "1.792249344530620");

   [Fact]
   public void Decimal128SequenceIsPinned()
      => AssertPinned<Decimal128>(
         "6.759434119590045364489583334943745",
         "8.144736480449251692093071142153349",
         "7.775129841449136547382181561260950",
         "9.308700010966428628478297468973324",
         "1.792249344530619792337233775469940");
}
#endif
