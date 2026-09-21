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
