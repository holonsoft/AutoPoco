using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

/// <summary>
///   DoubleToDecimal must reproduce the double to decimal conversion of .NET 8 to 10 bit for bit, scale
///   included: decimal equality ignores the scale, ToString does not. The pinned vectors were taken from
///   the .NET 10 runtime and hold on every runtime; on .NET 10 the runtime itself is the reference
///   for a large differential comparison.
/// </summary>
public class DoubleToDecimalTests {
   [Theory]
   [InlineData(0D, 0, 0, 0, 0)]                                         // 0
   [InlineData(1.1102230246251565E-16D, 2121462257, 258, 0, 1835008)]   // 0.0000000000000001110223024625, smallest sample of StableRandom
   [InlineData(0.1D, 1, 0, 0, 65536)]                                   // 0.1, .NET 11: 0.1000000000000000055511151231
   [InlineData(0.3333333333333333D, 921490773, 77610, 0, 983040)]       // 0.333333333333333
   [InlineData(0.25D, 25, 0, 0, 131072)]                                // 0.25
   [InlineData(0.5D, 5, 0, 0, 65536)]                                   // 0.5
   [InlineData(0.6759434119590045D, 1458914524, 157380, 0, 983040)]     // 0.675943411959004, not ...005: rounded after a floating point multiplication
   [InlineData(0.9999999999999999D, 10, 0, 0, 65536)]                   // 1.0, a sample just below 1 reaches 1
   [InlineData(0.12345678901234568D, -2045911174, 28744, 0, 983040)]    // 0.123456789012346
   [InlineData(1E-15D, 1, 0, 0, 983040)]                                // 0.000000000000001
   [InlineData(1E-20D, 1, 0, 0, 1310720)]                               // 0.00000000000000000001
   [InlineData(3E-28D, 3, 0, 0, 1835008)]                               // scale capped at 28
   [InlineData(4E-29D, 0, 0, 0, 0)]                                     // rounds to zero at scale 28
   [InlineData(1E-29D, 0, 0, 0, 0)]                                     // below the smallest exponent
   [InlineData(0.000123D, 123, 0, 0, 393216)]                           // trailing zeros factored out
   public void ConversionIsPinnedIncludingTheScale(double sample, int low, int mid, int high, int flags)
      => decimal.GetBits(DoubleToDecimal.FromSample(sample)).ShouldBe([low, mid, high, flags]);

   [Fact]
   public void ASampleJustBelowOneKeepsItsScaleInTheText()
      => DoubleToDecimal.FromSample(0.9999999999999999D).ToString(System.Globalization.CultureInfo.InvariantCulture).ShouldBe("1.0");

   [Theory]
   [InlineData(1D)]
   [InlineData(1.5D)]
   [InlineData(-0.1D)]
   [InlineData(double.NaN)]
   [InlineData(double.PositiveInfinity)]
   [InlineData(double.NegativeInfinity)]
   public void ASampleOutsideZeroToOneIsRejected(double sample) {
      var ex = Should.Throw<ArgumentOutOfRangeException>(() => DoubleToDecimal.FromSample(sample));
      ex.ParamName.ShouldBe("sample");
      ex.Message.ShouldContain("[0, 1)");
   }

   [Fact]
   public void NegativeZeroIsZero()
      => decimal.GetBits(DoubleToDecimal.FromSample(-0.0D)).ShouldBe([0, 0, 0, 0]);

#if !NET11_0_OR_GREATER
   // .NET 10 (like 8 and 9 before) still converts the old way, so the runtime is the reference implementation here.

   private static void ShouldMatchRuntime(double sample) {
      var actual = decimal.GetBits(DoubleToDecimal.FromSample(sample));
      var expected = decimal.GetBits((decimal) sample);

      // the message is only formatted on a mismatch, this runs a few million times
      if (!actual.AsSpan().SequenceEqual(expected))
         actual.ShouldBe(expected, $"sample {sample:R}");
   }

   [Fact]
   public void MatchesTheRuntimeOnRealSamples() {
      foreach (var seed in new[] { 0, 1, 1337, -1, int.MaxValue }) {
         var random = new StableRandom(seed);
         for (var i = 0; i < 400_000; i++)
            ShouldMatchRuntime(random.NextDouble());
      }
   }

   [Fact]
   public void MatchesTheRuntimeOverEveryExponent() {
      // arbitrary mantissas over every binary exponent below 1, down to the subnormals, which a uniform
      // sample hardly ever reaches: covers the capped scale and the rounding to zero
      var random = new StableRandom(42);
      for (var exponent = 0UL; exponent < 1023; exponent++)
         for (var i = 0; i < 500; i++) {
            var mantissa = random.NextUInt64() & ((1UL << 52) - 1);
            ShouldMatchRuntime(BitConverter.UInt64BitsToDouble((exponent << 52) | mantissa));
         }
   }

   [Fact]
   public void MatchesTheRuntimeNextToRoundingTies() {
      // values whose 15 digit scaling lands on or next to .5, where half to even and the double rounding matter
      var random = new StableRandom(7);
      for (var i = 0; i < 100_000; i++) {
         var tie = (random.NextInt64(100_000_000_000_000, 1_000_000_000_000_000) + 0.5) / 1e15;
         ShouldMatchRuntime(tie);
         ShouldMatchRuntime(Math.BitDecrement(tie));
         ShouldMatchRuntime(Math.BitIncrement(tie));
      }
   }

   [Theory]
   [InlineData(0D)]
   [InlineData(double.Epsilon)]
   [InlineData(0.9999999999999999D)]   // largest double below 1
   [InlineData(0.9999999999999998D)]   // the one below it
   [InlineData(0.5D)]
   [InlineData(1E-14D)]
   [InlineData(9.99999999999999E-15D)]
   public void MatchesTheRuntimeOnEdgeValues(double sample)
      => ShouldMatchRuntime(sample);

   [Fact]
   public void MatchesTheRuntimeOnNegativeZero()
      => ShouldMatchRuntime(-0.0D);
#endif
}
