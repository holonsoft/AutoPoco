namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Converts a random sample from double to decimal exactly the way .NET 8 to .NET 10 did, independent of
///   the runtime.
/// </summary>
/// <remarks>
///   Up to .NET 10 every double to decimal conversion (cast, constructor, <c>Convert.ToDecimal</c>,
///   <c>decimal.CreateChecked</c>) rounded to 15 significant digits. .NET 11 converts close to exactly, so
///   0.1 became 0.1000000000000000055511151231 and every decimal drawn from a double sample changed. AutoPoco
///   promises the same sequence for a seed on every runtime, so it brings the old conversion along instead
///   of depending on the runtime's.
///   <para>
///     Ported from <c>Decimal.DecCalc.VarDecFromR8</c> in dotnet/runtime, tag v10.0.0
///     (src/libraries/System.Private.CoreLib/src/System/Decimal.DecCalc.cs).
///     Copyright (c) .NET Foundation and Contributors. Licensed under the MIT license.
///     Only the branch for a sample in [0, 1) is ported, the scale-down branch for values of 1e15 and above
///     cannot be reached from there.
///   </para>
/// </remarks>
internal static class DoubleToDecimal {
   private const int MaxScale = 28;
   private const int DoubleBias = 1022;

   private static ReadOnlySpan<double> DoublePowers10 =>
   [
      1, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9,
      1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19,
      1e20, 1e21, 1e22, 1e23, 1e24, 1e25, 1e26, 1e27, 1e28,
   ];

   /// <summary>
   ///   Converts a sample in [0, 1) to decimal with the rounding to 15 significant digits of .NET 8 to 10,
   ///   bit for bit including the scale. Note that a sample just below 1 rounds up to exactly 1.
   /// </summary>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="sample" /> is not in [0, 1)</exception>
   public static decimal FromSample(double sample) {
      if (!(sample is >= 0 and < 1))
         throw new ArgumentOutOfRangeException(nameof(sample), sample, "The sample must lie in [0, 1).");

      // The most the decimal can scale by is 10^28, slightly more than 2^93. A double with a binary exponent
      // below -94 always rounds to zero, which includes zero itself.
      var exponent = (int) ((BitConverter.DoubleToUInt64Bits(sample) >> 52) & 0x7FF) - DoubleBias;
      if (exponent < -94)
         return 0m;

      // Highest power of 10 the value can have, from the binary exponent times log10(2) in 16 bit fixed
      // point (0.30103 * 65536 = 19728.3). For a sample below 1 the power is 14 or more, the value is scaled up
      // to a 15 digit integer.
      var power = 14 - ((exponent * 19728) >> 16);
      if (power > MaxScale)
         power = MaxScale;

      var scaled = sample * DoublePowers10[power];
      if (scaled < 1e14 && power < MaxScale) {
         scaled *= 10;
         power++;
      }

      // round half to even, what both the SSE4.1 and the fallback path of the runtime do
      var mantissa = (ulong) (long) Math.Round(scaled, MidpointRounding.ToEven);
      if (mantissa == 0)
         return 0m;

      // Factor out trailing zeros to reduce the scale, at most 14 of them (the leading digit of the 15 digit
      // number is not zero) and never more than the power used to get the integer. Same steps, same order
      // and same cheap pre-checks as the runtime, so the resulting scale is identical.
      var maxFactorOut = Math.Min(power, 14);

      if ((byte) mantissa == 0 && maxFactorOut >= 8)
         TryFactorOut(ref mantissa, ref power, ref maxFactorOut, 100_000_000, 8);

      if (((uint) mantissa & 0xF) == 0 && maxFactorOut >= 4)
         TryFactorOut(ref mantissa, ref power, ref maxFactorOut, 10_000, 4);

      if (((uint) mantissa & 3) == 0 && maxFactorOut >= 2)
         TryFactorOut(ref mantissa, ref power, ref maxFactorOut, 100, 2);

      if (((uint) mantissa & 1) == 0 && maxFactorOut >= 1)
         TryFactorOut(ref mantissa, ref power, ref maxFactorOut, 10, 1);

      return new decimal((int) (uint) mantissa, (int) (uint) (mantissa >> 32), 0, false, (byte) power);
   }

   private static void TryFactorOut(ref ulong mantissa, ref int power, ref int maxFactorOut, uint divisor, int digits) {
      var quotient = mantissa / divisor;

      // the runtime compares the low 32 bits only; kept on purpose, the mantissa has at most 16 digits
      if ((uint) mantissa != (uint) (quotient * divisor))
         return;

      mantissa = quotient;
      power -= digits;
      maxFactorOut -= digits;
   }
}
