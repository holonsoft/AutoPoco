using System.Buffers.Binary;
using System.Numerics;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Deterministic random number generator owned by AutoPoco: xoshiro256** seeded through SplitMix64.
///   The sequence for a seed is defined by this code alone, not by the .NET runtime, so test data stays
///   the same across runtime versions. Drop-in replacement for <see cref="Random" />, every public member
///   is overridden.
/// </summary>
public sealed class StableRandom : Random {
   private ulong _s0;
   private ulong _s1;
   private ulong _s2;
   private ulong _s3;

   public StableRandom(int seed)
      : this((long) seed) { }

   public StableRandom(long seed)
      : base(0) {
      var state = (ulong) seed;
      _s0 = SplitMix64(ref state);
      _s1 = SplitMix64(ref state);
      _s2 = SplitMix64(ref state);
      _s3 = SplitMix64(ref state);
   }

   /// <summary>
   ///   The raw 64 bit output of the generator.
   /// </summary>
   public ulong NextUInt64() {
      var result = BitOperations.RotateLeft(_s1 * 5, 7) * 9;
      var t = _s1 << 17;

      _s2 ^= _s0;
      _s3 ^= _s1;
      _s1 ^= _s2;
      _s0 ^= _s3;
      _s2 ^= t;
      _s3 = BitOperations.RotateLeft(_s3, 45);

      return result;
   }

   /// <inheritdoc />
   public override int Next() {
      int value;
      do
         value = (int) (NextUInt64() >> 33);
      while (value == int.MaxValue);

      return value;
   }

   /// <inheritdoc />
   public override int Next(int maxValue) {
      ArgumentOutOfRangeException.ThrowIfNegative(maxValue);
      return maxValue == 0 ? 0 : (int) NextBelow((ulong) maxValue);
   }

   /// <inheritdoc />
   public override int Next(int minValue, int maxValue) {
      if (minValue > maxValue)
         throw new ArgumentOutOfRangeException(nameof(minValue), minValue, "The minimum must not be greater than the maximum.");

      var range = (ulong) ((long) maxValue - minValue);
      return range == 0 ? minValue : (int) (minValue + (long) NextBelow(range));
   }

   /// <inheritdoc />
   public override long NextInt64()
      => (long) NextBelow(long.MaxValue);

   /// <inheritdoc />
   public override long NextInt64(long maxValue) {
      ArgumentOutOfRangeException.ThrowIfNegative(maxValue);
      return maxValue == 0 ? 0 : (long) NextBelow((ulong) maxValue);
   }

   /// <inheritdoc />
   public override long NextInt64(long minValue, long maxValue) {
      if (minValue > maxValue)
         throw new ArgumentOutOfRangeException(nameof(minValue), minValue, "The minimum must not be greater than the maximum.");

      var range = unchecked((ulong) (maxValue - minValue));
      return range == 0 ? minValue : unchecked(minValue + (long) NextBelow(range));
   }

   /// <inheritdoc />
   public override double NextDouble()
      => (NextUInt64() >> 11) * (1.0 / (1UL << 53));

   /// <inheritdoc />
   public override float NextSingle()
      => (NextUInt64() >> 40) * (1.0f / (1U << 24));

   /// <inheritdoc />
   public override void NextBytes(byte[] buffer) {
      ArgumentNullException.ThrowIfNull(buffer);
      NextBytes(buffer.AsSpan());
   }

   /// <inheritdoc />
   public override void NextBytes(Span<byte> buffer) {
      while (buffer.Length >= sizeof(ulong)) {
         BinaryPrimitives.WriteUInt64LittleEndian(buffer, NextUInt64());
         buffer = buffer[sizeof(ulong)..];
      }

      if (buffer.IsEmpty)
         return;

      Span<byte> rest = stackalloc byte[sizeof(ulong)];
      BinaryPrimitives.WriteUInt64LittleEndian(rest, NextUInt64());
      rest[..buffer.Length].CopyTo(buffer);
   }

   /// <inheritdoc />
   protected override double Sample()
      => NextDouble();

   /// <summary>
   ///   Uniform value in [0, bound) without modulo bias: mask to the next power of two and reject what is out of range.
   /// </summary>
   private ulong NextBelow(ulong bound) {
      var mask = ulong.MaxValue >> BitOperations.LeadingZeroCount((bound - 1) | 1);

      ulong candidate;
      do
         candidate = NextUInt64() & mask;
      while (candidate >= bound);

      return candidate;
   }

   private static ulong SplitMix64(ref ulong state) {
      state += 0x9E3779B97F4A7C15UL;
      var z = state;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
      return z ^ (z >> 31);
   }
}
