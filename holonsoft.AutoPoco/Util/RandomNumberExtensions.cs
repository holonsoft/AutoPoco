using System.Buffers.Binary;
using System.Numerics;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Uniform picks from a numeric range, shared by all numeric data sources.
/// </summary>
internal static class RandomNumberExtensions {
   extension(Random random) {
      /// <summary>
      ///   Uniform pick from the inclusive range of an integer type, both bounds can be produced. The width
      ///   of the range is computed in modular 128 bit arithmetic, which is exact for every integer type up
      ///   to 128 bit, signed or unsigned, including the whole range of the type.
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   <paramref name="max" /> is smaller than <paramref name="min" />
      /// </exception>
      public TNumber NextInclusive<TNumber>(TNumber min, TNumber max)
         where TNumber : INumber<TNumber> {
         ThrowIfMaxBelowMin(min, max);

         var width = unchecked(UInt128.CreateTruncating(max) - UInt128.CreateTruncating(min));

         UInt128 offset;
         if (width < long.MaxValue)
            offset = (UInt128) random.NextInt64(0, (long) width + 1);
         else if (width == UInt128.MaxValue)
            offset = NextUInt128(random);
         else
            offset = NextUInt128Below(random, width + 1);

         return unchecked(min + TNumber.CreateTruncating(offset));
      }

      /// <summary>
      ///   Interpolates between the bounds of a continuous type as <c>min * (1 - sample) + max * sample</c>
      ///   with a sample in [0, 1). Each product stays within the magnitude of its own bound, so the whole
      ///   range of the type is safe from overflow, unlike the obvious <c>min + sample * (max - min)</c>.
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   <paramref name="max" /> is smaller than <paramref name="min" />
      /// </exception>
      public TNumber NextBetween<TNumber>(TNumber min, TNumber max)
         where TNumber : INumber<TNumber> {
         ThrowIfMaxBelowMin(min, max);

         var sample = TNumber.CreateChecked(random.NextDouble());
         var value = (min * (TNumber.One - sample)) + (max * sample);
         return TNumber.Clamp(value, min, max);
      }
   }

   private static void ThrowIfMaxBelowMin<TNumber>(TNumber min, TNumber max)
      where TNumber : INumber<TNumber> {
      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum must not be smaller than the minimum {min}.");
   }

   private static UInt128 NextUInt128(Random random) {
      Span<byte> bytes = stackalloc byte[16];
      random.NextBytes(bytes);
      return BinaryPrimitives.ReadUInt128LittleEndian(bytes);
   }

   /// <summary>
   ///   Uniform value in [0, bound) without modulo bias, by rejecting the incomplete last block.
   /// </summary>
   private static UInt128 NextUInt128Below(Random random, UInt128 bound) {
      var excess = ((UInt128.MaxValue % bound) + 1) % bound;
      var highestAccepted = UInt128.MaxValue - excess;

      UInt128 candidate;
      do
         candidate = NextUInt128(random);
      while (candidate > highestAccepted);

      return candidate % bound;
   }
}
