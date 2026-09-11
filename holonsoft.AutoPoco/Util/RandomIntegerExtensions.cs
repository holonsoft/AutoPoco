using System.Buffers.Binary;
using System.Numerics;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Uniform picks from an inclusive integer range, shared by all integer data sources.
/// </summary>
internal static class RandomIntegerExtensions {
   extension(Random random) {
      /// <summary>
      ///   Uniform pick from the inclusive range, both bounds can be produced. The width of the range is
      ///   computed in modular 128 bit arithmetic, which is exact for every integer type up to 128 bit,
      ///   signed or unsigned, including the whole range of the type.
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   <paramref name="max" /> is smaller than <paramref name="min" />
      /// </exception>
      public TNumber NextInclusive<TNumber>(TNumber min, TNumber max)
         where TNumber : INumber<TNumber> {
         if (max < min)
            throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum must not be smaller than the minimum {min}.");

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
