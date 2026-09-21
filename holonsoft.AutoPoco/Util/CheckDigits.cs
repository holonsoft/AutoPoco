namespace holonsoft.AutoPoco.Util;

/// <summary>
///   The check digit algorithms behind the identifier data sources. They live here and not on a data source
///   because more than one source needs them: an ISBN-13 is a GTIN-13 and carries the same GS1 check digit.
/// </summary>
/// <remarks>
///   Both helpers trust their caller. They do not check that the span is long enough or that it holds digits
///   at all, a shorter span throws from the indexer and anything but a digit quietly produces a wrong result.
///   That is fine as long as they stay internal and the sources fill the span themselves.
/// </remarks>
internal static class CheckDigits {
   /// <summary>
   ///   The GS1 mod 10 check digit of a GTIN of any length, and of an ISBN-13. Counted from the right, the
   ///   data digits are weighted 3 and 1 alternating, the 3 falling on the digit directly left of the check
   ///   digit. The check digit is what lifts the weighted sum to the next multiple of ten.
   /// </summary>
   /// <param name="digits">The number, at least <paramref name="dataLength" /> characters long. Anything behind the data digits is ignored.</param>
   /// <param name="dataLength">How many leading characters are data digits, the check digit not counted.</param>
   public static int Gs1Mod10(ReadOnlySpan<char> digits, int dataLength) {
      var sum = 0;

      for (var i = 0; i < dataLength; i++) {
         var weight = (dataLength - 1 - i) % 2 == 0 ? 3 : 1;
         sum += (digits[i] - '0') * weight;
      }

      return (10 - (sum % 10)) % 10;
   }

   /// <summary>
   ///   The Luhn check digit, used by a credit card number, an IMEI and an ISIN. Counted from the right, the
   ///   data digits are doubled and undoubled alternating, starting with a doubled digit directly left of the
   ///   check digit, and a doubled value above nine has nine taken off it.
   /// </summary>
   /// <param name="digits">The number, at least <paramref name="dataLength" /> characters long.</param>
   /// <param name="dataLength">How many leading characters are data digits, the check digit not counted.</param>
   public static int Luhn(ReadOnlySpan<char> digits, int dataLength) {
      var sum = 0;
      // the digit directly left of the check digit is the first doubled one
      var doubled = true;

      for (var i = dataLength - 1; i >= 0; i--) {
         var digit = digits[i] - '0';

         if (doubled) {
            digit *= 2;
            if (digit > 9)
               digit -= 9;
         }

         sum += digit;
         doubled = !doubled;
      }

      return (10 - (sum % 10)) % 10;
   }

   /// <summary>
   ///   The mod 97 remainder of ISO/IEC 7064 as an IBAN uses it: the characters are read as one long number,
   ///   a letter counting as its position in the alphabet plus nine, so A becomes 10 and Z becomes 35.
   ///   For the two IBAN check digits the caller passes BBAN, country code and 00 and subtracts the remainder
   ///   from 98; a complete IBAN rearranged the same way is valid when the remainder is one.
   /// </summary>
   /// <param name="rearranged">Digits and capital letters only, in the rearranged IBAN order.</param>
   public static int Mod97(ReadOnlySpan<char> rearranged) {
      var remainder = 0;

      foreach (var c in rearranged) {
         if (c is >= '0' and <= '9') {
            remainder = (remainder * 10 + (c - '0')) % 97;
         } else {
            var value = c - 'A' + 10;
            remainder = (remainder * 10 + (value / 10)) % 97;
            remainder = (remainder * 10 + (value % 10)) % 97;
         }
      }

      return remainder;
   }

   /// <summary>
   ///   The MOD 11,10 hybrid of ISO/IEC 7064, used by the German VAT ID and the Croatian OIB: a running
   ///   product is folded through every data digit (sum mod 10, zero becomes ten, doubled mod 11), and the
   ///   check digit is what lifts the final product to eleven, where ten is written as zero.
   /// </summary>
   /// <param name="digits">The number, at least <paramref name="dataLength" /> characters long.</param>
   /// <param name="dataLength">How many leading characters are data digits, the check digit not counted.</param>
   public static int Iso7064Mod1110(ReadOnlySpan<char> digits, int dataLength) {
      var product = 10;

      for (var i = 0; i < dataLength; i++) {
         var sum = (digits[i] - '0' + product) % 10;
         if (sum == 0)
            sum = 10;

         product = 2 * sum % 11;
      }

      var check = 11 - product;
      return check == 10 ? 0 : check;
   }

   /// <summary>
   ///   A weighted sum mod 11, the check digit scheme of the Dutch and Polish VAT IDs: every data digit is
   ///   multiplied with its weight, and the check digit is the sum mod eleven.
   /// </summary>
   /// <param name="digits">The number, at least as many characters as <paramref name="weights" /> entries.</param>
   /// <param name="weights">One weight per data digit, the check digit not counted.</param>
   /// <returns>Zero to nine, or ten, which means the digits have no valid check digit and must be redrawn.</returns>
   public static int WeightedMod11(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights) {
      var sum = 0;

      for (var i = 0; i < weights.Length; i++)
         sum += (digits[i] - '0') * weights[i];

      return sum % 11;
   }

   /// <summary>
   ///   A mod 11 check digit over descending weights: the first data digit carries the highest weight, the
   ///   last one carries two, and the check digit makes the weighted sum a multiple of eleven.
   ///   An ISBN-10 uses it over nine data digits, so the weights run ten down to two. An ISSN uses it over
   ///   seven, so they run eight down to two.
   /// </summary>
   /// <param name="digits">The number, at least <paramref name="dataLength" /> characters long.</param>
   /// <param name="dataLength">How many leading characters are data digits, the check digit not counted.</param>
   /// <returns>Zero to nine, or ten, which an ISBN and an ISSN both write as an X.</returns>
   public static int Mod11Descending(ReadOnlySpan<char> digits, int dataLength) {
      var sum = 0;

      for (var i = 0; i < dataLength; i++)
         sum += (digits[i] - '0') * (dataLength + 1 - i);

      return (11 - (sum % 11)) % 11;
   }
}
