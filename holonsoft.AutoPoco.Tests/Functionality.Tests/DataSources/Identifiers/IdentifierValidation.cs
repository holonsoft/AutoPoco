namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

/// <summary>
///   Check digit validation for the identifier tests, written the other way round than the sources:
///   a source calculates the check digit from the data digits, this one weights the complete number and
///   asks whether the total comes out even. A weight applied in the wrong direction shows up as a
///   mismatch instead of cancelling out.
/// </summary>
internal static class IdentifierValidation {
   /// <summary>
   ///   True when the GS1 mod 10 check digit of a complete GTIN (or ISBN-13) is correct. The check digit
   ///   itself is weighted 1, the digit left of it 3, and so on alternating to the left.
   /// </summary>
   public static bool IsValidGtin(string? value)
      => value is not null && value.Length is 8 or 12 or 13 or 14 && HasValidGs1CheckDigit(value);

   /// <summary>
   ///   True when the check digit of a complete SSCC is correct. An SSCC is eighteen digits and carries the
   ///   same GS1 mod 10 check digit as a GTIN.
   /// </summary>
   public static bool IsValidSscc(string? value)
      => value is not null && value.Length == 18 && HasValidGs1CheckDigit(value);

   /// <summary>
   ///   The GS1 mod 10 check over a complete number of any length.
   /// </summary>
   private static bool HasValidGs1CheckDigit(string value) {
      var sum = 0;

      for (var i = 0; i < value.Length; i++) {
         if (value[i] is < '0' or > '9')
            return false;

         var weight = (value.Length - 1 - i) % 2 == 0 ? 1 : 3;
         sum += (value[i] - '0') * weight;
      }

      return sum % 10 == 0;
   }

   /// <summary>
   ///   True when the mod 11 check digit of a complete ISBN-10 is correct. The first character is weighted
   ///   10 down to the check digit weighted 1, an X as check digit counts as ten.
   /// </summary>
   public static bool IsValidIsbn10(string? value) {
      if (value is not { Length: 10 })
         return false;

      var sum = 0;

      for (var i = 0; i < 10; i++) {
         int digit;

         if (value[i] is >= '0' and <= '9')
            digit = value[i] - '0';
         else if (value[i] == 'X' && i == 9)
            digit = 10;
         else
            return false;

         sum += digit * (10 - i);
      }

      return sum % 11 == 0;
   }

   /// <summary>
   ///   True when the Luhn check digit of a complete number is correct. The check digit itself is weighted
   ///   one, the digit left of it is doubled, and so on alternating to the left. A doubled value above nine
   ///   has nine taken off it.
   /// </summary>
   public static bool IsValidLuhn(string? value) {
      if (string.IsNullOrEmpty(value))
         return false;

      var sum = 0;
      var doubled = false;

      for (var i = value.Length - 1; i >= 0; i--) {
         if (value[i] is < '0' or > '9')
            return false;

         var digit = value[i] - '0';

         if (doubled) {
            digit *= 2;
            if (digit > 9)
               digit -= 9;
         }

         sum += digit;
         doubled = !doubled;
      }

      return sum % 10 == 0;
   }

   /// <summary>
   ///   True when the mod 11 check character of a complete ISSN is correct. The first character is weighted
   ///   eight down to the check character weighted one, an X as check character counts as ten.
   /// </summary>
   public static bool IsValidIssn(string? value) {
      if (value is not { Length: 8 })
         return false;

      var sum = 0;

      for (var i = 0; i < 8; i++) {
         int digit;

         if (value[i] is >= '0' and <= '9')
            digit = value[i] - '0';
         else if (value[i] == 'X' && i == 7)
            digit = 10;
         else
            return false;

         sum += digit * (8 - i);
      }

      return sum % 11 == 0;
   }

   /// <summary>
   ///   True when the check digit of a complete ISIN is correct. Every letter is written out as its position
   ///   in the alphabet plus nine, and the Luhn check runs over the digits that come out of that.
   /// </summary>
   public static bool IsValidIsin(string? value) {
      if (value is not { Length: 12 })
         return false;

      if (value[0] is < 'A' or > 'Z' || value[1] is < 'A' or > 'Z')
         return false;

      var expanded = new System.Text.StringBuilder();

      foreach (var c in value) {
         if (c is >= '0' and <= '9')
            expanded.Append(c);
         else if (c is >= 'A' and <= 'Z')
            expanded.Append((c - 'A' + 10).ToString(System.Globalization.CultureInfo.InvariantCulture));
         else
            return false;
      }

      return IsValidLuhn(expanded.ToString());
   }

   /// <summary>
   ///   The BBAN shapes of the SEPA countries, written down independently of the source as regex patterns
   ///   from the SWIFT registry structures.
   /// </summary>
   private static readonly Dictionary<string, string> _ibanBbanPatterns = new() {
      ["AD"] = @"^\d{8}[0-9A-Z]{12}$",
      ["AT"] = @"^\d{16}$",
      ["BE"] = @"^\d{12}$",
      ["BG"] = @"^[A-Z]{4}\d{6}[0-9A-Z]{8}$",
      ["CH"] = @"^\d{5}[0-9A-Z]{12}$",
      ["CY"] = @"^\d{8}[0-9A-Z]{16}$",
      ["CZ"] = @"^\d{20}$",
      ["DE"] = @"^\d{18}$",
      ["DK"] = @"^\d{14}$",
      ["EE"] = @"^\d{16}$",
      ["ES"] = @"^\d{20}$",
      ["FI"] = @"^\d{14}$",
      ["FR"] = @"^\d{10}[0-9A-Z]{11}\d{2}$",
      ["GB"] = @"^[A-Z]{4}\d{14}$",
      ["GI"] = @"^[A-Z]{4}[0-9A-Z]{15}$",
      ["GR"] = @"^\d{7}[0-9A-Z]{16}$",
      ["HR"] = @"^\d{17}$",
      ["HU"] = @"^\d{24}$",
      ["IE"] = @"^[A-Z]{4}\d{14}$",
      ["IS"] = @"^\d{22}$",
      ["IT"] = @"^[A-Z]\d{10}[0-9A-Z]{12}$",
      ["LI"] = @"^\d{5}[0-9A-Z]{12}$",
      ["LT"] = @"^\d{16}$",
      ["LU"] = @"^\d{3}[0-9A-Z]{13}$",
      ["LV"] = @"^[A-Z]{4}[0-9A-Z]{13}$",
      ["MC"] = @"^\d{10}[0-9A-Z]{11}\d{2}$",
      ["MT"] = @"^[A-Z]{4}\d{5}[0-9A-Z]{18}$",
      ["NL"] = @"^[A-Z]{4}\d{10}$",
      ["NO"] = @"^\d{11}$",
      ["PL"] = @"^\d{24}$",
      ["PT"] = @"^\d{21}$",
      ["RO"] = @"^[A-Z]{4}[0-9A-Z]{16}$",
      ["SE"] = @"^\d{20}$",
      ["SI"] = @"^\d{15}$",
      ["SK"] = @"^\d{20}$",
      ["SM"] = @"^[A-Z]\d{10}[0-9A-Z]{12}$",
      ["VA"] = @"^\d{18}$",
   };

   /// <summary>
   ///   True when a complete IBAN in the electronic format has the right shape for its country, the mod 97
   ///   remainder one, and correct national check digits where the country keeps some. Validated the other
   ///   way round than the source: every national check is verified by adding the check digit into the
   ///   weighted sum instead of recalculating it, wherever the arithmetic allows that.
   /// </summary>
   public static bool IsValidIban(string? value) {
      if (value is null || value.Length < 5)
         return false;

      var country = value[..2];
      if (!_ibanBbanPatterns.TryGetValue(country, out var pattern))
         return false;

      if (value[2] is < '0' or > '9' || value[3] is < '0' or > '9')
         return false;

      var bban = value[4..];
      if (!System.Text.RegularExpressions.Regex.IsMatch(bban, pattern))
         return false;

      if (IbanMod97(bban + value[..4]) != 1)
         return false;

      return country switch {
         "BE" => IsValidBelgianBban(bban),
         "EE" => WeightedSumIsMultipleOf(bban, start: 2, dataWeights: [7, 1, 3], dataLength: 13, modulus: 10),
         "ES" => IsValidSpanishBban(bban),
         "FI" => IsValidLuhn(bban),
         "FR" or "MC" => IsValidRibKey(bban),
         "HU" => WeightedSumIsMultipleOf(bban, start: 0, dataWeights: [9, 7, 3, 1], dataLength: 7, modulus: 10)
                 && WeightedSumIsMultipleOf(bban, start: 8, dataWeights: [9, 7, 3, 1], dataLength: 15, modulus: 10),
         "IS" => WeightedSumIsMultipleOf(bban, start: 12, dataWeights: [3, 2, 7, 6, 5, 4, 3, 2], dataLength: 8, modulus: 11),
         "IT" or "SM" => IsValidItalianCin(bban),
         "NO" => WeightedSumIsMultipleOf(bban, start: 0, dataWeights: [5, 4, 3, 2, 7, 6, 5, 4, 3, 2], dataLength: 10, modulus: 11),
         "PL" => WeightedSumIsMultipleOf(bban, start: 0, dataWeights: [3, 9, 7, 1, 3, 9, 7], dataLength: 7, modulus: 10),
         "PT" => IbanMod97(bban) == 1,
         "SI" => IbanMod97(bban) == 1,
         _ => true
      };
   }

   /// <summary>
   ///   The mod 97 of ISO 7064 over an already rearranged string, letters counting as ten to thirty five.
   /// </summary>
   private static int IbanMod97(string rearranged) {
      var remainder = 0;

      foreach (var c in rearranged) {
         if (c is >= '0' and <= '9') {
            remainder = ((remainder * 10) + (c - '0')) % 97;
         } else {
            var value = c - 'A' + 10;
            remainder = ((remainder * 10) + (value / 10)) % 97;
            remainder = ((remainder * 10) + (value % 10)) % 97;
         }
      }

      return remainder;
   }

   /// <summary>
   ///   A weighted block sum with the check digit weighted one and added in, so a correct block lands on a
   ///   multiple of the modulus. The data weights repeat when the block is longer than they are, and the
   ///   check digit sits directly behind the data digits.
   /// </summary>
   private static bool WeightedSumIsMultipleOf(string bban, int start, int[] dataWeights, int dataLength, int modulus) {
      var sum = 0;

      for (var i = 0; i < dataLength; i++)
         sum += (bban[start + i] - '0') * dataWeights[i % dataWeights.Length];

      sum += bban[start + dataLength] - '0';

      return sum % modulus == 0;
   }

   private static bool IsValidBelgianBban(string bban) {
      var body = long.Parse(bban[..10], System.Globalization.CultureInfo.InvariantCulture);
      var check = int.Parse(bban[10..], System.Globalization.CultureInfo.InvariantCulture);

      var expected = (int) (body % 97);
      if (expected == 0)
         expected = 97;

      return check == expected;
   }

   private static bool IsValidSpanishBban(string bban) {
      int[] weights = [1, 2, 4, 8, 5, 10, 9, 7, 3, 6];

      var first = SpanishCheckDigit("00" + bban[..8], weights);
      var second = SpanishCheckDigit(bban[10..], weights);

      return bban[8] - '0' == first && bban[9] - '0' == second;
   }

   private static int SpanishCheckDigit(string digits, int[] weights) {
      var sum = 0;
      for (var i = 0; i < 10; i++)
         sum += (digits[i] - '0') * weights[i];

      var digit = 11 - (sum % 11);
      return digit switch {
         11 => 0,
         10 => 1,
         _ => digit
      };
   }

   private static bool IsValidRibKey(string bban) {
      var bank = long.Parse(bban[..5], System.Globalization.CultureInfo.InvariantCulture);
      var branch = long.Parse(bban[5..10], System.Globalization.CultureInfo.InvariantCulture);
      var key = long.Parse(bban[21..], System.Globalization.CultureInfo.InvariantCulture);

      var account = 0L;
      foreach (var c in bban[10..21])
         account = (account * 10) + c switch {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'I' => c - 'A' + 1,
            >= 'J' and <= 'R' => c - 'J' + 1,
            _ => c - 'S' + 2
         };

      return ((89 * bank) + (15 * branch) + (3 * account) + key) % 97 == 0;
   }

   private static bool IsValidItalianCin(string bban) {
      int[] oddValues = [1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23];
      var sum = 0;

      for (var i = 1; i < 23; i++) {
         var baseValue = bban[i] is >= '0' and <= '9'
            ? bban[i] - '0'
            : bban[i] - 'A';

         sum += i % 2 == 1
            ? oddValues[baseValue]
            : baseValue;
      }

      return bban[0] == 'A' + (sum % 26);
   }

   /// <summary>
   ///   True when a complete EU VAT ID with its country prefix carries a correct check digit. Only the
   ///   countries the source generates are known here.
   /// </summary>
   public static bool IsValidVatId(string? value) {
      if (value is null || value.Length < 4)
         return false;

      return value[..2] switch {
         "AT" => IsValidAustrianVatId(value),
         "DE" => IsValidGermanVatId(value),
         "HR" => IsValidCroatianVatId(value),
         "IT" => IsValidItalianVatId(value),
         "NL" => IsValidDutchVatId(value),
         "PL" => IsValidPolishVatId(value),
         _ => false
      };
   }

   /// <summary>
   ///   ATU and eight digits. Validated the other way round than the generator: the check digit is added to
   ///   the weighted digit sum, and the total has to land on 96 mod 10, i.e. on 6.
   /// </summary>
   private static bool IsValidAustrianVatId(string value) {
      if (value.Length != 11 || value[2] != 'U')
         return false;

      var sum = 0;

      for (var i = 0; i < 7; i++) {
         var c = value[3 + i];
         if (c is < '0' or > '9')
            return false;

         var digit = (c - '0') * (i % 2 == 0 ? 1 : 2);
         if (digit > 9)
            digit = (digit / 10) + (digit % 10);

         sum += digit;
      }

      if (value[10] is < '0' or > '9')
         return false;

      return (sum + (value[10] - '0')) % 10 == 6;
   }

   /// <summary>
   ///   DE and nine digits, the first one no zero. Validated with the closure property of ISO 7064 MOD 11,10:
   ///   running the product chain over the complete number, check digit included, has to end on a sum of one.
   /// </summary>
   private static bool IsValidGermanVatId(string value)
      => value.Length == 11 && value[2] != '0' && RunsIso7064Mod1110ToOne(value.AsSpan(2));

   /// <summary>
   ///   HR and the eleven digit OIB, the same closure property as the German number.
   /// </summary>
   private static bool IsValidCroatianVatId(string value)
      => value.Length == 13 && RunsIso7064Mod1110ToOne(value.AsSpan(2));

   /// <summary>
   ///   IT and eleven digits: the Luhn check over the complete number and an office code between 001 and 100.
   /// </summary>
   private static bool IsValidItalianVatId(string value) {
      if (value.Length != 13 || !IsValidLuhn(value[2..]))
         return false;

      var office = int.Parse(value.AsSpan(9, 3), System.Globalization.CultureInfo.InvariantCulture);
      return office is >= 1 and <= 100;
   }

   /// <summary>
   ///   NL, nine digits, a B and a two digit suffix that is not 00. Validated with the check digit weighted
   ///   minus one, so the weighted sum over all nine digits has to be divisible by eleven.
   /// </summary>
   private static bool IsValidDutchVatId(string value) {
      if (value.Length != 14 || value[11] != 'B')
         return false;

      if (value[12] is < '0' or > '9' || value[13] is < '0' or > '9' || (value[12] == '0' && value[13] == '0'))
         return false;

      var sum = 0;

      for (var i = 0; i < 9; i++) {
         var c = value[2 + i];
         if (c is < '0' or > '9')
            return false;

         sum += (c - '0') * (i < 8 ? 9 - i : -1);
      }

      return sum % 11 == 0;
   }

   /// <summary>
   ///   PL and the ten digit NIP, the first digit no zero, the check digit weighted minus one so the weighted
   ///   sum over all ten digits has to be divisible by eleven.
   /// </summary>
   private static bool IsValidPolishVatId(string value) {
      if (value.Length != 12 || value[2] == '0')
         return false;

      ReadOnlySpan<int> weights = [6, 5, 7, 2, 3, 4, 5, 6, 7, -1];
      var sum = 0;

      for (var i = 0; i < 10; i++) {
         var c = value[2 + i];
         if (c is < '0' or > '9')
            return false;

         sum += (c - '0') * weights[i];
      }

      return sum % 11 == 0;
   }

   /// <summary>
   ///   The closure property of ISO 7064 MOD 11,10: fold every digit into the running product, and a number
   ///   whose check digit is correct ends with a folded sum of exactly one.
   /// </summary>
   private static bool RunsIso7064Mod1110ToOne(ReadOnlySpan<char> digits) {
      var product = 10;
      var sum = 0;

      foreach (var c in digits) {
         if (c is < '0' or > '9')
            return false;

         sum = (c - '0' + product) % 10;
         if (sum == 0)
            sum = 10;

         product = 2 * sum % 11;
      }

      return sum == 1;
   }

   /// <summary>
   ///   Every single character variation of the value at the given position, the original excluded.
   ///   An ISBN-10 may carry an X in its last position, everything else is a digit.
   /// </summary>
   public static IEnumerable<string> SingleCharacterMutations(string value, bool allowTrailingX) {
      for (var position = 0; position < value.Length; position++) {
         var alphabet = allowTrailingX && position == value.Length - 1
            ? "0123456789X"
            : "0123456789";

         foreach (var c in alphabet) {
            if (c == value[position])
               continue;

            var mutated = value.ToCharArray();
            mutated[position] = c;
            yield return new string(mutated);
         }
      }
   }
}
