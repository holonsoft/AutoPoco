using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an IBAN with correct check digits in the electronic format without spaces,
///   e.g. <c>DE89370400440532013000</c>. Every SEPA country is supported, and where the country keeps its
///   own check digits inside the account part, those are calculated as well, so the number passes a
///   validator that knows the national rules, not only the mod 97 check.
/// </summary>
/// <remarks>
///   The structures come from the SWIFT IBAN registry (ISO 13616). The national check digits are
///   implemented for BE, EE, ES, FI, FR, HU, IS, IT, MC, NO, PL, PT, SI and SM; DK and SE carry a check
///   digit convention inside the account for which no reliably published algorithm exists, there the digit
///   is drawn like any other. The Polish check covers the bank code, the Icelandic one sits inside the
///   kennitala of the account holder.
///   The numbers are syntactically valid. The bank codes are drawn, so a number can name a bank that
///   exists, and an IBAN needs no second factor for a SEPA direct debit, so treat these numbers with the
///   same care as real ones and keep them inside your test systems. For data that leaves your machine use
///   <see cref="TestBlzIbanSource" />, whose bank code provably belongs to no bank.
/// </remarks>
public abstract class IbanSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   One country of the IBAN registry: the BBAN as segments, e.g. <c>1a5n5n12c</c> for Italy, and the
   ///   calculation of the national check digits where the country has some. A calculation returns false
   ///   when the drawn digits allow no valid check digit, the BBAN is then drawn again.
   /// </summary>
   private sealed record CountrySpec(string BbanSegments, Func<char[], bool>? NationalCheck = null) {
      public (int Length, char Kind)[] Segments { get; } = ParseSegments(BbanSegments);
      public int BbanLength { get; } = ParseSegments(BbanSegments).Sum(s => s.Length);

      private static (int Length, char Kind)[] ParseSegments(string spec) {
         var segments = new List<(int, char)>();
         var length = 0;

         foreach (var c in spec)
            if (c is >= '0' and <= '9')
               length = length * 10 + (c - '0');
            else {
               segments.Add((length, c));
               length = 0;
            }

         return [.. segments];
      }
   }

   /// <summary>
   ///   The SEPA countries of the SWIFT IBAN registry. n stands for digits, a for capital letters,
   ///   c for digits and capital letters.
   /// </summary>
   private static readonly Dictionary<string, CountrySpec> _registry = new() {
      ["AD"] = new("4n4n12c"),
      ["AT"] = new("5n11n"),
      ["BE"] = new("3n7n2n", FixBelgianCheck),
      ["BG"] = new("4a4n2n8c"),
      ["CH"] = new("5n12c"),
      ["CY"] = new("3n5n16c"),
      ["CZ"] = new("4n6n10n"),
      ["DE"] = new("8n10n"),
      ["DK"] = new("4n9n1n"),
      ["EE"] = new("2n2n11n1n", FixEstonianCheck),
      ["ES"] = new("4n4n2n10n", FixSpanishCheck),
      ["FI"] = new("3n11n", FixFinnishCheck),
      ["FR"] = new("5n5n11c2n", FixRibKey),
      ["GB"] = new("4a6n8n"),
      ["GI"] = new("4a15c"),
      ["GR"] = new("3n4n16c"),
      ["HR"] = new("7n10n"),
      ["HU"] = new("3n4n1n15n1n", FixHungarianChecks),
      ["IE"] = new("4a6n8n"),
      ["IS"] = new("4n2n6n10n", FixIcelandicCheck),
      ["IT"] = new("1a5n5n12c", FixItalianCin),
      ["LI"] = new("5n12c"),
      ["LT"] = new("5n11n"),
      ["LU"] = new("3n13c"),
      ["LV"] = new("4a13c"),
      ["MC"] = new("5n5n11c2n", FixRibKey),
      ["MT"] = new("4a5n18c"),
      ["NL"] = new("4a10n"),
      ["NO"] = new("4n6n1n", FixNorwegianCheck),
      ["PL"] = new("8n16n", FixPolishBankCodeCheck),
      ["PT"] = new("4n4n11n2n", FixPortugueseCheck),
      ["RO"] = new("4a16c"),
      ["SE"] = new("3n16n1n"),
      ["SI"] = new("5n8n2n", FixSlovenianCheck),
      ["SK"] = new("4n6n10n"),
      ["SM"] = new("1a5n5n12c", FixItalianCin),
      ["VA"] = new("3n15n"),
   };

   private static readonly string[] _countryCodes = [.. _registry.Keys.OrderBy(c => c, StringComparer.Ordinal)];

   /// <summary>
   ///   The countries this source can generate, as ISO 3166-1 alpha-2 codes.
   /// </summary>
   public static IReadOnlyCollection<string> SupportedCountries => _countryCodes;

   private const string _alphanumericAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

   private readonly string? _fixedCountry;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="countryCode">
   ///   Optional ISO 3166-1 alpha-2 country code, e.g. <c>DE</c>. Drawn from <see cref="SupportedCountries" />
   ///   when null.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="countryCode" /> is not a SEPA country of the registry.</exception>
   protected IbanSourceBase(string? countryCode = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (countryCode is not null) {
         if (!_registry.ContainsKey(countryCode))
            throw new ArgumentException(
               $"'{countryCode}' is not a SEPA country this source can generate an IBAN for. Supported: {string.Join(", ", _countryCodes)}.",
               nameof(countryCode));

         _fixedCountry = countryCode;
      }
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var country = _fixedCountry ?? _countryCodes[Random.Next(0, _countryCodes.Length)];
      var spec = _registry[country];
      var bban = new char[spec.BbanLength];

      do {
         var position = 0;

         foreach (var (length, kind) in spec.Segments)
            for (var i = 0; i < length; i++)
               bban[position++] = kind switch {
                  'n' => (char) ('0' + Random.Next(0, 10)),
                  'a' => (char) ('A' + Random.Next(0, 26)),
                  _ => _alphanumericAlphabet[Random.Next(0, _alphanumericAlphabet.Length)]
               };
      } while (spec.NationalCheck is not null && !spec.NationalCheck(bban));

      return BuildIban(country, bban);
   }

   /// <summary>
   ///   Puts the two ISO 7064 mod 97 check digits between country code and BBAN.
   /// </summary>
   private protected static string BuildIban(string countryCode, char[] bban) {
      Span<char> rearranged = stackalloc char[bban.Length + 4];
      bban.CopyTo(rearranged);
      countryCode.AsSpan().CopyTo(rearranged[bban.Length..]);
      rearranged[bban.Length + 2] = '0';
      rearranged[bban.Length + 3] = '0';

      var check = 98 - CheckDigits.Mod97(rearranged);

      return $"{countryCode}{check:00}{new string(bban)}";
   }

   /// <summary>
   ///   Belgium closes its twelve digits with the first ten mod 97, a remainder of zero written as 97.
   /// </summary>
   private static bool FixBelgianCheck(char[] bban) {
      var check = (int) (DigitsAsNumber(bban, 0, 10) % 97);
      if (check == 0)
         check = 97;

      WriteTwoDigits(bban, 10, check);
      return true;
   }

   /// <summary>
   ///   Estonia checks the thirteen digits behind the two digit bank prefix with the weights 7, 1, 3
   ///   repeating; the last digit lifts the weighted sum to the next multiple of ten.
   /// </summary>
   private static bool FixEstonianCheck(char[] bban) {
      ReadOnlySpan<int> weights = [7, 1, 3];
      var sum = 0;

      for (var i = 2; i < 15; i++)
         sum += (bban[i] - '0') * weights[(i - 2) % 3];

      bban[15] = (char) ('0' + ((10 - (sum % 10)) % 10));
      return true;
   }

   /// <summary>
   ///   Spain carries two check digits in the middle: one over 00 plus bank and branch, one over the ten
   ///   account digits, both weighted 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 with 11 minus the sum mod 11, where
   ///   eleven becomes zero and ten becomes one.
   /// </summary>
   private static bool FixSpanishCheck(char[] bban) {
      Span<char> padded = stackalloc char[10];
      padded[0] = '0';
      padded[1] = '0';
      bban.AsSpan(0, 8).CopyTo(padded[2..]);

      bban[8] = (char) ('0' + SpanishDigit(padded));
      bban[9] = (char) ('0' + SpanishDigit(bban.AsSpan(10, 10)));
      return true;
   }

   private static int SpanishDigit(ReadOnlySpan<char> digits) {
      ReadOnlySpan<int> weights = [1, 2, 4, 8, 5, 10, 9, 7, 3, 6];
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

   /// <summary>
   ///   Finland closes its fourteen digits with the Luhn check digit of the first thirteen.
   /// </summary>
   private static bool FixFinnishCheck(char[] bban) {
      bban[13] = (char) ('0' + CheckDigits.Luhn(bban, 13));
      return true;
   }

   /// <summary>
   ///   France and Monaco close with the RIB key: 97 minus 89 times the bank, 15 times the branch and
   ///   3 times the account, mod 97. A letter in the account counts by the RIB table, A to I as 1 to 9,
   ///   J to R again as 1 to 9 and S to Z as 2 to 9.
   /// </summary>
   private static bool FixRibKey(char[] bban) {
      var bank = DigitsAsNumber(bban, 0, 5);
      var branch = DigitsAsNumber(bban, 5, 5);

      var account = 0L;
      for (var i = 10; i < 21; i++)
         account = account * 10 + RibValue(bban[i]);

      var key = 97 - (int) (((89 * bank) + (15 * branch) + (3 * account)) % 97);

      WriteTwoDigits(bban, 21, key);
      return true;
   }

   private static int RibValue(char c)
      => c switch {
         >= '0' and <= '9' => c - '0',
         >= 'A' and <= 'I' => c - 'A' + 1,
         >= 'J' and <= 'R' => c - 'J' + 1,
         _ => c - 'S' + 2
      };

   /// <summary>
   ///   Hungary carries two check digits: the eighth digit checks bank and branch, the last one checks the
   ///   fifteen account digits, both with the weights 9, 7, 3, 1 repeating and the digit that lifts the sum
   ///   to the next multiple of ten.
   /// </summary>
   private static bool FixHungarianChecks(char[] bban) {
      bban[7] = (char) ('0' + HungarianDigit(bban.AsSpan(0, 7)));
      bban[23] = (char) ('0' + HungarianDigit(bban.AsSpan(8, 15)));
      return true;
   }

   private static int HungarianDigit(ReadOnlySpan<char> digits) {
      ReadOnlySpan<int> weights = [9, 7, 3, 1];
      var sum = 0;

      for (var i = 0; i < digits.Length; i++)
         sum += (digits[i] - '0') * weights[i % 4];

      return (10 - (sum % 10)) % 10;
   }

   /// <summary>
   ///   The last ten digits of an Icelandic BBAN are the kennitala of the account holder, whose ninth digit
   ///   is a mod 11 check over the first eight with the weights 3, 2, 7, 6, 5, 4, 3, 2. A remainder of one
   ///   allows no check digit, those digits are drawn again.
   /// </summary>
   private static bool FixIcelandicCheck(char[] bban) {
      ReadOnlySpan<int> weights = [3, 2, 7, 6, 5, 4, 3, 2];
      var sum = 0;

      // the kennitala fills the last ten positions of the 22 digit BBAN, so it starts at twelve
      for (var i = 0; i < 8; i++)
         sum += (bban[12 + i] - '0') * weights[i];

      var remainder = sum % 11;
      if (remainder == 1)
         return false;

      bban[20] = (char) ('0' + ((11 - remainder) % 11));
      return true;
   }

   /// <summary>
   ///   Italy and San Marino start the BBAN with the CIN letter over the twenty two characters behind it:
   ///   a character at an even position counts its base value, digits as themselves and letters as zero to
   ///   twenty five, one at an odd position counts by the CIN table, and the sum mod 26 names the letter.
   /// </summary>
   private static bool FixItalianCin(char[] bban) {
      ReadOnlySpan<int> oddValues = [1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23];
      var sum = 0;

      for (var i = 1; i < 23; i++) {
         var baseValue = bban[i] is >= '0' and <= '9'
            ? bban[i] - '0'
            : bban[i] - 'A';

         // the first character behind the CIN is position one, an odd position
         sum += i % 2 == 1
            ? oddValues[baseValue]
            : baseValue;
      }

      bban[0] = (char) ('A' + (sum % 26));
      return true;
   }

   /// <summary>
   ///   Norway closes its eleven digits with a mod 11 check over the first ten, weighted 5, 4, 3, 2, 7, 6,
   ///   5, 4, 3, 2. A remainder of one allows no check digit, those digits are drawn again.
   /// </summary>
   private static bool FixNorwegianCheck(char[] bban) {
      ReadOnlySpan<int> weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
      var sum = 0;

      for (var i = 0; i < 10; i++)
         sum += (bban[i] - '0') * weights[i];

      var remainder = sum % 11;
      if (remainder == 1)
         return false;

      bban[10] = (char) ('0' + ((11 - remainder) % 11));
      return true;
   }

   /// <summary>
   ///   The eighth digit of a Polish bank code checks the seven before it, weighted 3, 9, 7, 1, 3, 9, 7,
   ///   with the digit that lifts the sum to the next multiple of ten. The sixteen account digits carry no
   ///   check of their own.
   /// </summary>
   private static bool FixPolishBankCodeCheck(char[] bban) {
      ReadOnlySpan<int> weights = [3, 9, 7, 1, 3, 9, 7];
      var sum = 0;

      for (var i = 0; i < 7; i++)
         sum += (bban[i] - '0') * weights[i];

      bban[7] = (char) ('0' + ((10 - (sum % 10)) % 10));
      return true;
   }

   /// <summary>
   ///   Portugal closes its NIB with two ISO 7064 mod 97 digits over the nineteen digits before them.
   /// </summary>
   private static bool FixPortugueseCheck(char[] bban) {
      WriteTwoDigits(bban, 19, 98 - Mod97OfDigitsTimesHundred(bban.AsSpan(0, 19)));
      return true;
   }

   /// <summary>
   ///   Slovenia closes with two ISO 7064 mod 97 digits over the thirteen digits before them.
   /// </summary>
   private static bool FixSlovenianCheck(char[] bban) {
      WriteTwoDigits(bban, 13, 98 - Mod97OfDigitsTimesHundred(bban.AsSpan(0, 13)));
      return true;
   }

   private static int Mod97OfDigitsTimesHundred(ReadOnlySpan<char> digits) {
      Span<char> padded = stackalloc char[digits.Length + 2];
      digits.CopyTo(padded);
      padded[^2] = '0';
      padded[^1] = '0';

      return CheckDigits.Mod97(padded);
   }

   private static long DigitsAsNumber(char[] bban, int start, int length) {
      var value = 0L;

      for (var i = start; i < start + length; i++)
         value = value * 10 + (bban[i] - '0');

      return value;
   }

   private static void WriteTwoDigits(char[] bban, int position, int value) {
      bban[position] = (char) ('0' + (value / 10));
      bban[position + 1] = (char) ('0' + (value % 10));
   }
}

/// <summary>
///   An IBAN of a SEPA country with correct check digits, national ones included.
/// </summary>
public class IbanSource(string? countryCode) : IbanSourceBase(countryCode) {
   public IbanSource() : this(null) { }
}

/// <summary>
///   An IBAN of a SEPA country that returns null every now and then.
/// </summary>
public class NullableIbanSource(string? countryCode, int nullCreationThreshold) : IbanSourceBase(countryCode, nullCreationThreshold) {
   public NullableIbanSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIbanSource(string? countryCode) : this(countryCode, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIbanSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A German IBAN on a bank code that provably belongs to no bank, so the number is recognisable as test
///   data and cannot name anybody's account.
/// </summary>
/// <remarks>
///   A German bank code starts with its clearing area, and the Bundesbank hands out the areas one to eight
///   (Merkblatt Bankleitzahlendatei; in the official bank code file of September 2026 not one of the 13760
///   codes starts with a nine). This source therefore starts every bank code with a nine. The mod 97 check
///   digits are correct, so the number passes any structural IBAN validation, and a lookup of the bank code
///   comes back empty, which is the point. An officially reserved test bank code does not exist in Germany,
///   this convention is the closest safe equivalent.
///   Austria and Switzerland publish no unassigned range, so no such source exists for them.
/// </remarks>
public abstract class TestBlzIbanSourceBase : DataSourceBase<string> {
   protected TestBlzIbanSourceBase(int? nullCreationThreshold = null)
      : base(nullCreationThreshold) { }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var bban = new char[18];
      bban[0] = '9';

      for (var i = 1; i < 18; i++)
         bban[i] = (char) ('0' + Random.Next(0, 10));

      Span<char> rearranged = stackalloc char[22];
      bban.CopyTo(rearranged);
      "DE00".AsSpan().CopyTo(rearranged[18..]);

      var check = 98 - CheckDigits.Mod97(rearranged);

      return $"DE{check:00}{new string(bban)}";
   }
}

/// <summary>
///   A German IBAN on a bank code outside the assigned range, recognisable as test data.
/// </summary>
public class TestBlzIbanSource() : TestBlzIbanSourceBase(null);

/// <summary>
///   A German IBAN on a bank code outside the assigned range that returns null every now and then.
/// </summary>
public class NullableTestBlzIbanSource(int nullCreationThreshold) : TestBlzIbanSourceBase(nullCreationThreshold) {
   public NullableTestBlzIbanSource() : this(AutoPocoDefaults.NullCreationThreshold) { }
}
