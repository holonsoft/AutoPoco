using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates a VAT identification number with a valid check digit, the country prefix included,
///   e.g. <c>DE249051813</c> or <c>ATU13585627</c>.
/// </summary>
/// <remarks>
///   Only countries whose check digit algorithm is published and calibrated against an official worked
///   example or a registered real number are drawn, so every generated number passes a validator that knows
///   the national rules, not just the format. The construction rules of most countries come from the
///   BMF document "Konstruktionsregeln der UID" (November 2020), Croatia from the OIB specification.
///   Greece, Spain, France, Cyprus, Czechia, Latvia, Malta, Bulgaria and Romania have not agreed to a
///   publication of their rules, so they stay out on purpose: a number that only looks right is worse test
///   data than a missing country. XI is Northern Ireland, which uses the United Kingdom scheme and is the
///   one GB style prefix VIES still knows.
///   The numbers are syntactically valid, they are not registered and do not identify a real company. A VIES
///   lookup will therefore report them as not registered, which is the correct answer for test data.
/// </remarks>
public abstract class VatIdSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   The countries this source can generate, as the prefixes the numbers carry.
   /// </summary>
   public static IReadOnlyCollection<string> SupportedCountries { get; } =
      ["AT", "BE", "DE", "DK", "EE", "FI", "GB", "HR", "HU", "IE", "IT", "LT", "LU", "NL", "PL", "PT", "SE", "SI", "SK", "XI"];

   private static readonly string[] _countries = [.. SupportedCountries];

   /// <summary>
   ///   The weights of the Dutch check digit, applied to the first eight of the nine digits.
   /// </summary>
   private static readonly int[] _dutchWeights = [9, 8, 7, 6, 5, 4, 3, 2];

   /// <summary>
   ///   The weights of the Polish check digit, applied to the first nine of the ten digits.
   /// </summary>
   private static readonly int[] _polishWeights = [6, 5, 7, 2, 3, 4, 5, 6, 7];

   private readonly string? _fixedCountry;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="countryCode">
   ///   Optional ISO 3166-1 alpha-2 country code, e.g. <c>DE</c>. Drawn from <see cref="SupportedCountries" />
   ///   when null.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="countryCode" /> is not a supported country.</exception>
   protected VatIdSourceBase(string? countryCode = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (countryCode is not null) {
         if (!SupportedCountries.Contains(countryCode))
            throw new ArgumentException(
               $"'{countryCode}' is not a country this source can generate a VAT ID for. Supported: {string.Join(", ", SupportedCountries)}.",
               nameof(countryCode));

         _fixedCountry = countryCode;
      }
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var country = _fixedCountry ?? _countries[Random.Next(0, _countries.Length)];

      return country switch {
         "AT" => NextAustrianId(),
         "BE" => NextBelgianId(),
         "DE" => NextGermanId(),
         "DK" => NextDanishId(),
         "EE" => NextEstonianId(),
         "FI" => NextFinnishId(),
         "GB" or "XI" => NextUnitedKingdomId(country),
         "HR" => NextCroatianId(),
         "HU" => NextHungarianId(),
         "IE" => NextIrishId(),
         "IT" => NextItalianId(),
         "LT" => NextLithuanianId(),
         "LU" => NextLuxembourgishId(),
         "NL" => NextDutchId(),
         "PL" => NextPolishId(),
         "PT" => NextPortugueseId(),
         "SE" => NextSwedishId(),
         "SI" => NextSlovenianId(),
         "SK" => NextSlovakId(),
         _ => throw new InvalidOperationException($"Country '{country}' has no VAT ID generator.")
      };
   }

   /// <summary>
   ///   BE and ten digits: a leading zero or one, seven digits of enterprise number, and the pair 97 minus
   ///   the first eight digits mod 97. The second digit stays off zero, which both the old and the new
   ///   Belgian scheme allow.
   /// </summary>
   private string NextBelgianId() {
      Span<char> digits = stackalloc char[10];
      digits[0] = (char) ('0' + Random.Next(0, 2));
      digits[1] = (char) ('1' + Random.Next(0, 9));

      for (var i = 2; i < 8; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var body = 0;
      for (var i = 0; i < 8; i++)
         body = (body * 10) + (digits[i] - '0');

      var check = 97 - (body % 97);
      digits[8] = (char) ('0' + (check / 10));
      digits[9] = (char) ('0' + (check % 10));

      return $"BE{new string(digits)}";
   }

   /// <summary>
   ///   DK and eight digits, the first one off zero. The whole number, weighted 2, 7, 6, 5, 4, 3, 2, 1, has
   ///   to be divisible by eleven; when no last digit achieves that, the first seven are drawn again.
   /// </summary>
   private string NextDanishId() {
      ReadOnlySpan<int> weights = [2, 7, 6, 5, 4, 3, 2];
      Span<char> digits = stackalloc char[8];
      int check;

      do {
         digits[0] = (char) ('1' + Random.Next(0, 9));

         for (var i = 1; i < 7; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         var sum = 0;
         for (var i = 0; i < 7; i++)
            sum += (digits[i] - '0') * weights[i];

         check = (11 - (sum % 11)) % 11;
      } while (check == 10);

      digits[7] = (char) ('0' + check);

      return $"DK{new string(digits)}";
   }

   /// <summary>
   ///   EE and nine digits that always start with 10. The weights 3, 7, 1 repeat over the first eight, and
   ///   the last digit lifts the sum to the next multiple of ten.
   /// </summary>
   private string NextEstonianId() {
      ReadOnlySpan<int> weights = [3, 7, 1];
      Span<char> digits = stackalloc char[9];
      digits[0] = '1';
      digits[1] = '0';

      for (var i = 2; i < 8; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var sum = 0;
      for (var i = 0; i < 8; i++)
         sum += (digits[i] - '0') * weights[i % 3];

      digits[8] = (char) ('0' + ((10 - (sum % 10)) % 10));

      return $"EE{new string(digits)}";
   }

   /// <summary>
   ///   FI and eight digits, the last one the mod 11 check over the unusual weights 7, 9, 10, 5, 8, 4, 2.
   ///   A weighted sum with remainder one allows no check digit, those digits are drawn again.
   /// </summary>
   private string NextFinnishId() {
      ReadOnlySpan<int> weights = [7, 9, 10, 5, 8, 4, 2];
      Span<char> digits = stackalloc char[8];
      int remainder;

      do {
         for (var i = 0; i < 7; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         var sum = 0;
         for (var i = 0; i < 7; i++)
            sum += (digits[i] - '0') * weights[i];

         remainder = sum % 11;
      } while (remainder == 1);

      digits[7] = (char) ('0' + ((11 - remainder) % 11));

      return $"FI{new string(digits)}";
   }

   /// <summary>
   ///   GB or XI and nine digits: seven digits weighted 8 down to 2, closed by the two digit remainder that
   ///   brings the sum to a multiple of 97, either directly (the MOD 97 scheme) or after adding 55
   ///   (the MOD 9755 scheme for newer numbers). Each scheme excludes a few ranges of the leading seven
   ///   digits, a draw inside them is repeated.
   /// </summary>
   private string NextUnitedKingdomId(string prefix) {
      ReadOnlySpan<int> weights = [8, 7, 6, 5, 4, 3, 2];
      Span<char> digits = stackalloc char[9];
      var mod9755 = Random.Next(0, 2) == 1;
      int body;

      do {
         body = Random.Next(0, 10_000_000);
      } while (mod9755
         ? body is >= 1 and <= 1_000_000
         : body is >= 100_000 and <= 999_999 or >= 9_490_001 and <= 9_700_000 or >= 9_990_001 and <= 9_999_999);

      for (var i = 6; i >= 0; i--) {
         digits[i] = (char) ('0' + (body % 10));
         body /= 10;
      }

      var sum = mod9755 ? 55 : 0;
      for (var i = 0; i < 7; i++)
         sum += (digits[i] - '0') * weights[i];

      var check = (97 - (sum % 97)) % 97;
      digits[7] = (char) ('0' + (check / 10));
      digits[8] = (char) ('0' + (check % 10));

      return $"{prefix}{new string(digits)}";
   }

   /// <summary>
   ///   HU and eight digits, the last one lifting the sum over the weights 9, 7, 3, 1, 9, 7, 3 to the next
   ///   multiple of ten. The first digit stays off zero.
   /// </summary>
   private string NextHungarianId() {
      ReadOnlySpan<int> weights = [9, 7, 3, 1, 9, 7, 3];
      Span<char> digits = stackalloc char[8];
      digits[0] = (char) ('1' + Random.Next(0, 9));

      for (var i = 1; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var sum = 0;
      for (var i = 0; i < 7; i++)
         sum += (digits[i] - '0') * weights[i];

      digits[7] = (char) ('0' + ((10 - (sum % 10)) % 10));

      return $"HU{new string(digits)}";
   }

   /// <summary>
   ///   The letters the Irish check maps a remainder to: zero is a W, one to twenty two are A to V.
   /// </summary>
   private const string _irishCheckCharacters = "WABCDEFGHIJKLMNOPQRSTUV";

   /// <summary>
   ///   IE and seven digits weighted 8 down to 2, closed by a check letter mod 23. Half of the numbers get
   ///   the nine character form with a trailing letter A to I, which is weighted nine into the sum.
   /// </summary>
   private string NextIrishId() {
      Span<char> digits = stackalloc char[7];

      for (var i = 0; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var sum = 0;
      for (var i = 0; i < 7; i++)
         sum += (digits[i] - '0') * (8 - i);

      if (Random.Next(0, 2) == 0)
         return $"IE{new string(digits)}{_irishCheckCharacters[sum % 23]}";

      var trailing = (char) ('A' + Random.Next(0, 9));
      sum += (trailing - 'A' + 1) * 9;

      return $"IE{new string(digits)}{_irishCheckCharacters[sum % 23]}{trailing}";
   }

   /// <summary>
   ///   LT and nine digits, the eighth always a one. The check digit is the sum over the weights 1 to 8
   ///   mod 11; when that is ten, a second pass over the weights 3 to 9, 1 decides, and a ten there
   ///   becomes a zero. No draw is ever invalid.
   /// </summary>
   private string NextLithuanianId() {
      Span<char> digits = stackalloc char[9];

      for (var i = 0; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[7] = '1';

      var first = 0;
      for (var i = 0; i < 8; i++)
         first += (digits[i] - '0') * (i + 1);

      var check = first % 11;

      if (check == 10) {
         var second = 0;
         for (var i = 0; i < 8; i++)
            second += (digits[i] - '0') * (((i + 2) % 9) + 1);

         check = second % 11;
         if (check == 10)
            check = 0;
      }

      digits[8] = (char) ('0' + check);

      return $"LT{new string(digits)}";
   }

   /// <summary>
   ///   LU and eight digits: six digits of registration number, closed by their value mod 89 as a pair.
   /// </summary>
   private string NextLuxembourgishId() {
      Span<char> digits = stackalloc char[8];

      var body = 0;
      for (var i = 0; i < 6; i++) {
         digits[i] = (char) ('0' + Random.Next(0, 10));
         body = (body * 10) + (digits[i] - '0');
      }

      var check = body % 89;
      digits[6] = (char) ('0' + (check / 10));
      digits[7] = (char) ('0' + (check % 10));

      return $"LU{new string(digits)}";
   }

   /// <summary>
   ///   PT and nine digits, the first one off zero, the last one 11 minus the sum over the weights 9 down
   ///   to 2 mod 11, where ten and eleven both become zero. No draw is ever invalid.
   /// </summary>
   private string NextPortugueseId() {
      Span<char> digits = stackalloc char[9];
      digits[0] = (char) ('1' + Random.Next(0, 9));

      for (var i = 1; i < 8; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var sum = 0;
      for (var i = 0; i < 8; i++)
         sum += (digits[i] - '0') * (9 - i);

      var check = 11 - (sum % 11);
      if (check >= 10)
         check = 0;

      digits[8] = (char) ('0' + check);

      return $"PT{new string(digits)}";
   }

   /// <summary>
   ///   SE and twelve digits: nine digits of organisation number, their Luhn check digit, and a suffix
   ///   between 01 and 94, in practice almost always 01.
   /// </summary>
   private string NextSwedishId() {
      Span<char> digits = stackalloc char[12];

      for (var i = 0; i < 9; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[9] = (char) ('0' + CheckDigits.Luhn(digits, 9));

      var suffix = 1 + Random.Next(0, 94);
      digits[10] = (char) ('0' + (suffix / 10));
      digits[11] = (char) ('0' + (suffix % 10));

      return $"SE{new string(digits)}";
   }

   /// <summary>
   ///   SI and eight digits, the first one off zero, the last one 11 minus the sum over the weights 8 down
   ///   to 2 mod 11, where ten becomes zero and a sum divisible by eleven allows no check digit, those
   ///   digits are drawn again.
   /// </summary>
   private string NextSlovenianId() {
      Span<char> digits = stackalloc char[8];
      int remainder;

      do {
         digits[0] = (char) ('1' + Random.Next(0, 9));

         for (var i = 1; i < 7; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         var sum = 0;
         for (var i = 0; i < 7; i++)
            sum += (digits[i] - '0') * (8 - i);

         remainder = sum % 11;
      } while (remainder == 0);

      var check = 11 - remainder;
      digits[7] = (char) ('0' + (check == 10 ? 0 : check));

      return $"SI{new string(digits)}";
   }

   /// <summary>
   ///   SK and ten digits: the first one off zero, the third one a 2, 3, 4, 7, 8 or 9, and the whole number
   ///   divisible by eleven. When no last digit achieves that, the first nine are drawn again.
   /// </summary>
   private string NextSlovakId() {
      const string thirdDigits = "234789";
      Span<char> digits = stackalloc char[10];
      int check;

      do {
         digits[0] = (char) ('1' + Random.Next(0, 9));
         digits[1] = (char) ('0' + Random.Next(0, 10));
         digits[2] = thirdDigits[Random.Next(0, thirdDigits.Length)];

         for (var i = 3; i < 9; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         var remainder = 0;
         for (var i = 0; i < 9; i++)
            remainder = ((remainder * 10) + (digits[i] - '0')) % 11;

         check = (11 - ((remainder * 10) % 11)) % 11;
      } while (check == 10);

      digits[9] = (char) ('0' + check);

      return $"SK{new string(digits)}";
   }

   /// <summary>
   ///   ATU, seven digits and the BMF check digit: the digits are weighted 1 and 2 alternating, a doubled
   ///   value keeps its digit sum, and the check digit is 96 minus the sum, mod 10.
   /// </summary>
   private string NextAustrianId() {
      Span<char> digits = stackalloc char[8];

      for (var i = 0; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var sum = 0;
      for (var i = 0; i < 7; i++) {
         var value = (digits[i] - '0') * (i % 2 == 0 ? 1 : 2);
         if (value > 9)
            value = (value / 10) + (value % 10);

         sum += value;
      }

      digits[7] = (char) ('0' + ((96 - sum) % 10));

      return $"ATU{new string(digits)}";
   }

   /// <summary>
   ///   DE and nine digits, the first one never a zero, the last one the ISO 7064 MOD 11,10 check digit.
   /// </summary>
   private string NextGermanId() {
      Span<char> digits = stackalloc char[9];
      digits[0] = (char) ('1' + Random.Next(0, 9));

      for (var i = 1; i < 8; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[8] = (char) ('0' + CheckDigits.Iso7064Mod1110(digits, 8));

      return $"DE{new string(digits)}";
   }

   /// <summary>
   ///   HR and the eleven digit OIB, the last digit the same ISO 7064 MOD 11,10 check digit a German ID uses.
   /// </summary>
   private string NextCroatianId() {
      Span<char> digits = stackalloc char[11];

      for (var i = 0; i < 10; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[10] = (char) ('0' + CheckDigits.Iso7064Mod1110(digits, 10));

      return $"HR{new string(digits)}";
   }

   /// <summary>
   ///   IT and eleven digits: seven digits of sequential number, a three digit office code inside the range
   ///   that is actually handed out (001 to 100), and the Luhn check digit.
   /// </summary>
   private string NextItalianId() {
      Span<char> digits = stackalloc char[11];

      for (var i = 0; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var office = 1 + Random.Next(0, 100);
      digits[7] = (char) ('0' + (office / 100));
      digits[8] = (char) ('0' + (office / 10 % 10));
      digits[9] = (char) ('0' + (office % 10));

      digits[10] = (char) ('0' + CheckDigits.Luhn(digits, 10));

      return $"IT{new string(digits)}";
   }

   /// <summary>
   ///   NL, nine digits, a B and a two digit suffix from 01 on. The ninth digit is the weighted mod 11 check
   ///   digit of the first eight; a draw whose weighted sum leaves remainder ten has no valid check digit,
   ///   so those eight digits are drawn again.
   /// </summary>
   private string NextDutchId() {
      Span<char> digits = stackalloc char[9];
      int check;

      do {
         for (var i = 0; i < 8; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         check = CheckDigits.WeightedMod11(digits, _dutchWeights);
      } while (check == 10);

      digits[8] = (char) ('0' + check);

      var suffix = 1 + Random.Next(0, 99);

      return $"NL{new string(digits)}B{suffix:00}";
   }

   /// <summary>
   ///   PL and the ten digit NIP, the first digit never a zero, the last one the weighted mod 11 check digit
   ///   of the first nine. A draw whose weighted sum leaves remainder ten is drawn again.
   /// </summary>
   private string NextPolishId() {
      Span<char> digits = stackalloc char[10];
      int check;

      do {
         digits[0] = (char) ('1' + Random.Next(0, 9));

         for (var i = 1; i < 9; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         check = CheckDigits.WeightedMod11(digits, _polishWeights);
      } while (check == 10);

      digits[9] = (char) ('0' + check);

      return $"PL{new string(digits)}";
   }
}

/// <summary>
///   An EU VAT identification number with a valid check digit.
/// </summary>
public class VatIdSource(string? countryCode) : VatIdSourceBase(countryCode) {
   public VatIdSource() : this(null) { }
}

/// <summary>
///   An EU VAT identification number that returns null every now and then.
/// </summary>
public class NullableVatIdSource(string? countryCode, int nullCreationThreshold) : VatIdSourceBase(countryCode, nullCreationThreshold) {
   public NullableVatIdSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableVatIdSource(string? countryCode) : this(countryCode, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableVatIdSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
