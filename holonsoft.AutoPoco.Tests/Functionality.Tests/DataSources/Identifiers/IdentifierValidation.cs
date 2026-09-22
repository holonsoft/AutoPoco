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
         "BE" => IsValidBelgianVatId(value),
         "DE" => IsValidGermanVatId(value),
         "DK" => IsValidDanishVatId(value),
         "EE" => IsValidEstonianVatId(value),
         "FI" => IsValidFinnishVatId(value),
         "GB" or "XI" => IsValidUnitedKingdomVatId(value),
         "HR" => IsValidCroatianVatId(value),
         "HU" => IsValidHungarianVatId(value),
         "IE" => IsValidIrishVatId(value),
         "IT" => IsValidItalianVatId(value),
         "LT" => IsValidLithuanianVatId(value),
         "LU" => IsValidLuxembourgishVatId(value),
         "NL" => IsValidDutchVatId(value),
         "PL" => IsValidPolishVatId(value),
         "PT" => IsValidPortugueseVatId(value),
         "SE" => IsValidSwedishVatId(value),
         "SI" => IsValidSlovenianVatId(value),
         "SK" => IsValidSlovakVatId(value),
         _ => false
      };
   }

   private static bool AllDigits(ReadOnlySpan<char> value) {
      foreach (var c in value)
         if (c is < '0' or > '9')
            return false;

      return true;
   }

   /// <summary>
   ///   BE and ten digits starting with a zero or a one: the last two are 97 minus the first eight mod 97.
   /// </summary>
   private static bool IsValidBelgianVatId(string value) {
      if (value.Length != 12 || !AllDigits(value.AsSpan(2)) || value[2] is not ('0' or '1'))
         return false;

      var body = int.Parse(value.AsSpan(2, 8), System.Globalization.CultureInfo.InvariantCulture);
      var check = int.Parse(value.AsSpan(10, 2), System.Globalization.CultureInfo.InvariantCulture);

      return check == 97 - (body % 97);
   }

   /// <summary>
   ///   DK and eight digits, the first off zero: weighted 2, 7, 6, 5, 4, 3, 2, 1 the number is divisible
   ///   by eleven.
   /// </summary>
   private static bool IsValidDanishVatId(string value) {
      if (value.Length != 10 || !AllDigits(value.AsSpan(2)) || value[2] == '0')
         return false;

      ReadOnlySpan<int> weights = [2, 7, 6, 5, 4, 3, 2, 1];
      var sum = 0;

      for (var i = 0; i < 8; i++)
         sum += (value[2 + i] - '0') * weights[i];

      return sum % 11 == 0;
   }

   /// <summary>
   ///   EE and nine digits starting with 10: weighted 3, 7, 1 repeating with the check digit added, the sum
   ///   is a multiple of ten.
   /// </summary>
   private static bool IsValidEstonianVatId(string value) {
      if (value.Length != 11 || !AllDigits(value.AsSpan(2)) || value[2] != '1' || value[3] != '0')
         return false;

      ReadOnlySpan<int> weights = [3, 7, 1];
      var sum = value[10] - '0';

      for (var i = 0; i < 8; i++)
         sum += (value[2 + i] - '0') * weights[i % 3];

      return sum % 10 == 0;
   }

   /// <summary>
   ///   FI and eight digits: weighted 7, 9, 10, 5, 8, 4, 2 with the check digit added, the sum is a
   ///   multiple of eleven.
   /// </summary>
   private static bool IsValidFinnishVatId(string value) {
      if (value.Length != 10 || !AllDigits(value.AsSpan(2)))
         return false;

      ReadOnlySpan<int> weights = [7, 9, 10, 5, 8, 4, 2];
      var sum = value[9] - '0';

      for (var i = 0; i < 7; i++)
         sum += (value[2 + i] - '0') * weights[i];

      return sum % 11 == 0;
   }

   /// <summary>
   ///   GB or XI and nine digits: the sum over the weights 8 down to 2 plus the two digit tail is a
   ///   multiple of 97, either directly or after adding 55, and the leading seven digits avoid the ranges
   ///   the matching scheme excludes.
   /// </summary>
   private static bool IsValidUnitedKingdomVatId(string value) {
      if (value.Length != 11 || !AllDigits(value.AsSpan(2)))
         return false;

      var sum = 0;
      for (var i = 0; i < 7; i++)
         sum += (value[2 + i] - '0') * (8 - i);

      sum += int.Parse(value.AsSpan(9, 2), System.Globalization.CultureInfo.InvariantCulture);
      var body = int.Parse(value.AsSpan(2, 7), System.Globalization.CultureInfo.InvariantCulture);

      if (sum % 97 == 0)
         return body is (< 100_000 or > 999_999) and (< 9_490_001 or > 9_700_000) and (< 9_990_001 or > 9_999_999);

      if ((sum + 55) % 97 == 0)
         return body is 0 or > 1_000_000;

      return false;
   }

   /// <summary>
   ///   HU and eight digits: weighted 9, 7, 3, 1, 9, 7, 3 with the check digit added, the sum is a
   ///   multiple of ten. The official rules name no constraint on the first digit, so none is checked.
   /// </summary>
   private static bool IsValidHungarianVatId(string value) {
      if (value.Length != 10 || !AllDigits(value.AsSpan(2)))
         return false;

      ReadOnlySpan<int> weights = [9, 7, 3, 1, 9, 7, 3];
      var sum = value[9] - '0';

      for (var i = 0; i < 7; i++)
         sum += (value[2 + i] - '0') * weights[i];

      return sum % 10 == 0;
   }

   /// <summary>
   ///   IE and seven digits with a check letter mod 23, either as the eight character form or the nine
   ///   character form whose trailing letter A to I is weighted nine into the sum.
   /// </summary>
   private static bool IsValidIrishVatId(string value) {
      const string checkCharacters = "WABCDEFGHIJKLMNOPQRSTUV";

      if (value.Length is not (10 or 11) || !AllDigits(value.AsSpan(2, 7)))
         return false;

      var sum = 0;
      for (var i = 0; i < 7; i++)
         sum += (value[2 + i] - '0') * (8 - i);

      if (value.Length == 11) {
         if (value[10] is < 'A' or > 'I')
            return false;

         sum += (value[10] - 'A' + 1) * 9;
      }

      return value[9] == checkCharacters[sum % 23];
   }

   /// <summary>
   ///   LT and nine digits with an eighth digit of one, or twelve digits with an eleventh digit of one.
   ///   The check digit comes from the first weight pass mod 11, or from the second pass when the first
   ///   lands on ten, a ten there counting as zero.
   /// </summary>
   private static bool IsValidLithuanianVatId(string value) {
      if (value.Length is not (11 or 14) || !AllDigits(value.AsSpan(2)))
         return false;

      var dataLength = value.Length - 3;

      if (value[2 + dataLength - 1] != '1')
         return false;

      var first = 0;
      for (var i = 0; i < dataLength; i++)
         first += (value[2 + i] - '0') * ((i % 9) + 1);

      var check = first % 11;

      if (check == 10) {
         var second = 0;
         for (var i = 0; i < dataLength; i++)
            second += (value[2 + i] - '0') * (((i + 2) % 9) + 1);

         check = second % 11;
         if (check == 10)
            check = 0;
      }

      return value[^1] - '0' == check;
   }

   /// <summary>
   ///   LU and eight digits: the last two are the first six mod 89.
   /// </summary>
   private static bool IsValidLuxembourgishVatId(string value) {
      if (value.Length != 10 || !AllDigits(value.AsSpan(2)))
         return false;

      var body = int.Parse(value.AsSpan(2, 6), System.Globalization.CultureInfo.InvariantCulture);
      var check = int.Parse(value.AsSpan(8, 2), System.Globalization.CultureInfo.InvariantCulture);

      return check == body % 89;
   }

   /// <summary>
   ///   PT and nine digits, the first off zero: the check digit is 11 minus the sum over the weights 9 down
   ///   to 2 mod 11, a ten or eleven written as zero.
   /// </summary>
   private static bool IsValidPortugueseVatId(string value) {
      if (value.Length != 11 || !AllDigits(value.AsSpan(2)) || value[2] == '0')
         return false;

      var sum = 0;
      for (var i = 0; i < 8; i++)
         sum += (value[2 + i] - '0') * (9 - i);

      var check = 11 - (sum % 11);
      if (check >= 10)
         check = 0;

      return value[10] - '0' == check;
   }

   /// <summary>
   ///   SE and twelve digits: the first ten pass the Luhn check and the suffix lies between 01 and 94.
   /// </summary>
   private static bool IsValidSwedishVatId(string value) {
      if (value.Length != 14 || !AllDigits(value.AsSpan(2)))
         return false;

      if (!IsValidLuhn(value[2..12]))
         return false;

      var suffix = int.Parse(value.AsSpan(12, 2), System.Globalization.CultureInfo.InvariantCulture);
      return suffix is >= 1 and <= 94;
   }

   /// <summary>
   ///   SI and eight digits, the first off zero: the check digit is 11 minus the sum over the weights 8
   ///   down to 2 mod 11, a ten written as zero and a sum divisible by eleven allowed no number at all.
   /// </summary>
   private static bool IsValidSlovenianVatId(string value) {
      if (value.Length != 10 || !AllDigits(value.AsSpan(2)) || value[2] == '0')
         return false;

      var sum = 0;
      for (var i = 0; i < 7; i++)
         sum += (value[2 + i] - '0') * (8 - i);

      var remainder = sum % 11;
      if (remainder == 0)
         return false;

      var check = 11 - remainder;
      if (check == 10)
         check = 0;

      return value[9] - '0' == check;
   }

   /// <summary>
   ///   SK and ten digits, the first off zero and the third a 2, 3, 4, 7, 8 or 9: the whole number is
   ///   divisible by eleven.
   /// </summary>
   private static bool IsValidSlovakVatId(string value) {
      if (value.Length != 12 || !AllDigits(value.AsSpan(2)) || value[2] == '0' || value[4] is not ('2' or '3' or '4' or '7' or '8' or '9'))
         return false;

      var remainder = 0;
      foreach (var c in value.AsSpan(2))
         remainder = ((remainder * 10) + (c - '0')) % 11;

      return remainder == 0;
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
   ///   True when an EORI number is well formed for its country and its national part carries the check
   ///   digits of the underlying national identifier. Only the countries the source generates are known.
   /// </summary>
   public static bool IsValidEori(string? value) {
      if (value is null || value.Length < 4)
         return false;

      var country = value[..2];
      var national = value[2..];

      return country switch {
         // the Belgian enterprise number is the Belgian VAT number without the prefix
         "BE" => national.Length == 10 && IsValidVatId($"BE{national}"),
         "DE" => national.Length == 15 && AllDigits(national),
         // the CVR number is the Danish VAT number without the prefix
         "DK" => national.Length == 8 && IsValidVatId($"DK{national}"),
         "FR" => IsValidSiret(national),
         // the VAT registration number with three zeros behind it
         "GB" or "XI" => national.Length == 12 && national.EndsWith("000", StringComparison.Ordinal)
                         && IsValidVatId($"GB{national[..9]}"),
         // the OIB is the Croatian VAT number without the prefix
         "HR" => national.Length == 11 && IsValidVatId($"HR{national}"),
         // the partita IVA is the Italian VAT number without the prefix
         "IT" => national.Length == 11 && IsValidVatId($"IT{national}"),
         "NL" => national.Length == 9 && AllDigits(national),
         // the NIP with five zeros behind it
         "PL" => national.Length == 15 && national.EndsWith("00000", StringComparison.Ordinal)
                 && IsValidVatId($"PL{national[..10]}"),
         _ => false
      };
   }

   /// <summary>
   ///   True when a fourteen digit SIRET is valid: the whole number passes the Luhn check and so does the
   ///   nine digit SIREN it starts with.
   /// </summary>
   private static bool IsValidSiret(string national)
      => national.Length == 14 && AllDigits(national) && IsValidLuhn(national) && IsValidLuhn(national[..9]);

   /// <summary>
   ///   True when a seventeen character VIN carries the correct North American check digit at position nine.
   ///   Validated the other way round than the generator: every character is transliterated and weighted,
   ///   the check position with weight zero, and the sum mod 11 has to name the check character, a ten
   ///   written as an X.
   /// </summary>
   public static bool IsValidVin(string? value) {
      const string alphabet = "0123456789ABCDEFGHJKLMNPRSTUVWXYZ";
      ReadOnlySpan<int> weights = [8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2];

      if (value is not { Length: 17 })
         return false;

      var sum = 0;

      for (var i = 0; i < 17; i++) {
         if (!alphabet.Contains(value[i]))
            return false;

         sum += VinTransliterationValue(value[i]) * weights[i];
      }

      var expected = sum % 11;
      var actual = value[8] switch {
         >= '0' and <= '9' => value[8] - '0',
         'X' => 10,
         _ => -1
      };

      return actual == expected;
   }

   private static int VinTransliterationValue(char c)
      => c switch {
         >= '0' and <= '9' => c - '0',
         >= 'A' and <= 'H' => c - 'A' + 1,
         >= 'J' and <= 'N' => c - 'J' + 1,
         'P' => 7,
         'R' => 9,
         _ => c - 'S' + 2
      };

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
