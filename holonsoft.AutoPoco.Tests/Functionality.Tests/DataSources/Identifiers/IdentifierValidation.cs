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
