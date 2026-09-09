using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

/// <summary>
///   The pinned values are the contract: the sequence of a seed must never change, whatever the runtime.
/// </summary>
public class StableRandomTests {
   private static List<T> Draw<T>(int count, Func<StableRandom, T> next, int seed = 1337) {
      var random = new StableRandom(seed);
      return Enumerable.Range(0, count).Select(_ => next(random)).ToList();
   }

   [Fact]
   public void RawOutputIsPinned()
      => Draw(4, r => r.NextUInt64()).ShouldBe([12468955128717782748UL, 15024386940265324015UL, 14342583032507416131UL, 17171520676123500948UL]);

   [Fact]
   public void NextIsPinned() {
      Draw(6, r => r.Next()).ShouldBe([1451577424, 1749068840, 1669696419, 1999028105, 384882616, 1405532988]);
      Draw(3, r => r.Next(), seed: 0).ShouldBe([1291202459, 1605832636, 221233742]);
      Draw(3, r => r.Next(), seed: -1).ShouldBe([1202360426, 1648054284, 1089411296]);
   }

   [Fact]
   public void BoundedNextIsPinned() {
      Draw(10, r => r.Next(100)).ShouldBe([92, 67, 20, 49, 83, 96, 23, 92, 66, 98]);
      Draw(10, r => r.Next(-50, 50)).ShouldBe([42, 17, -30, -1, 33, 46, -27, 42, 16, 48]);
   }

   [Fact]
   public void NextInt64IsPinned()
      => Draw(4, r => r.NextInt64()).ShouldBe([3245583091863006940L, 5801014903410548207L, 5119210995652640323L, 7948148639268725140L]);

   [Fact]
   public void FloatingPointIsPinned() {
      Draw(4, r => r.NextDouble()).ShouldBe([0.6759434119590045D, 0.8144736480449252D, 0.7775129841449137D, 0.9308700010966429D]);
      Draw(4, r => r.NextSingle()).ShouldBe([0.6759434F, 0.8144736F, 0.77751297F, 0.93087F]);
   }

   [Fact]
   public void NextBytesIsPinnedAndFillsPartialBlocks() {
      var random = new StableRandom(1337);
      var bytes = new byte[11];

      random.NextBytes(bytes);

      bytes.ShouldBe([220, 46, 130, 79, 160, 160, 10, 173, 239, 93, 136]);
   }

   [Fact]
   public void NextBytesOnASpanMatchesTheArrayOverload() {
      var expected = new byte[13];
      new StableRandom(5).NextBytes(expected);
      Span<byte> span = stackalloc byte[13];

      new StableRandom(5).NextBytes(span);

      span.ToArray().ShouldBe(expected);
   }

   [Fact]
   public void SameSeedSameSequenceDifferentSeedDifferentSequence() {
      Draw(20, r => r.Next(), 42).ShouldBe(Draw(20, r => r.Next(), 42));
      Draw(20, r => r.Next(), 42).ShouldNotBe(Draw(20, r => r.Next(), 43));
      Draw(20, r => r.Next(), int.MinValue).ShouldNotBe(Draw(20, r => r.Next(), int.MaxValue));
   }

   [Fact]
   public void NextStaysBelowIntMaxValue()
      => Draw(10_000, r => r.Next()).ShouldAllBe(x => x >= 0 && x < int.MaxValue);

   [Fact]
   public void BoundedValuesStayInRange() {
      Draw(10_000, r => r.Next(7)).ShouldAllBe(x => x >= 0 && x < 7);
      Draw(10_000, r => r.Next(-3, 4)).ShouldAllBe(x => x >= -3 && x < 4);
      Draw(10_000, r => r.Next(int.MinValue, int.MaxValue)).Count.ShouldBe(10_000);
      Draw(10_000, r => r.NextInt64(9)).ShouldAllBe(x => x >= 0 && x < 9);
      Draw(10_000, r => r.NextInt64(long.MinValue, long.MaxValue)).Count.ShouldBe(10_000);
      Draw(10_000, r => r.NextInt64()).ShouldAllBe(x => x >= 0);
      Draw(10_000, r => r.NextDouble()).ShouldAllBe(x => x >= 0.0 && x < 1.0);
      Draw(10_000, r => r.NextSingle()).ShouldAllBe(x => x >= 0.0f && x < 1.0f);
   }

   [Fact]
   public void EmptyRangesReturnTheMinimum() {
      new StableRandom(1).Next(0).ShouldBe(0);
      new StableRandom(1).Next(5, 5).ShouldBe(5);
      new StableRandom(1).NextInt64(0).ShouldBe(0);
      new StableRandom(1).NextInt64(-9, -9).ShouldBe(-9);
   }

   [Fact]
   public void FullRangesReachBothSigns() {
      Draw(1000, r => r.Next(int.MinValue, int.MaxValue)).ShouldContain(x => x < 0);
      Draw(1000, r => r.Next(int.MinValue, int.MaxValue)).ShouldContain(x => x > 0);
      Draw(1000, r => r.NextInt64(long.MinValue, long.MaxValue)).ShouldContain(x => x < int.MinValue);
      Draw(1000, r => r.NextInt64(long.MinValue, long.MaxValue)).ShouldContain(x => x > int.MaxValue);
   }

   [Fact]
   public void SmallRangesAreRoughlyUniform() {
      var histogram = Draw(100_000, r => r.Next(10)).GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

      histogram.Keys.Order().ShouldBe(Enumerable.Range(0, 10));
      histogram.Values.ShouldAllBe(count => count > 9_500 && count < 10_500);
   }

   [Fact]
   public void ARangeJustAbovePowerOfTwoIsRoughlyUniform() {
      // 129 forces rejection sampling with a 255 mask
      var histogram = Draw(129_000, r => r.Next(129)).GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

      histogram.Count.ShouldBe(129);
      histogram.Values.ShouldAllBe(count => count > 800 && count < 1200);
   }

   [Fact]
   public void InvalidArgumentsThrow() {
      var random = new StableRandom(1);

      Should.Throw<ArgumentOutOfRangeException>(() => random.Next(-1));
      Should.Throw<ArgumentOutOfRangeException>(() => random.Next(2, 1));
      Should.Throw<ArgumentOutOfRangeException>(() => random.NextInt64(-1));
      Should.Throw<ArgumentOutOfRangeException>(() => random.NextInt64(2, 1));
      Should.Throw<ArgumentNullException>(() => random.NextBytes((byte[]) null!));
   }

   [Fact]
   public void ShuffleUsesTheStableStream() {
      var first = Enumerable.Range(0, 20).ToArray();
      var second = Enumerable.Range(0, 20).ToArray();

      new StableRandom(8).Shuffle(first);
      new StableRandom(8).Shuffle(second);

      first.ShouldBe(second);
      first.ShouldNotBe(Enumerable.Range(0, 20));
   }
}
