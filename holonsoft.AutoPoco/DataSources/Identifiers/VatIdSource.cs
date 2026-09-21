using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an EU VAT identification number with a valid check digit, the country prefix included,
///   e.g. <c>DE249051813</c> or <c>ATU13585627</c>.
/// </summary>
/// <remarks>
///   Only countries whose check digit algorithm this source implements are drawn, so every generated number
///   passes a validator that knows the national rules, not just the format. Currently: Germany and Croatia
///   (ISO 7064 MOD 11,10), Austria (the BMF digit sum with its 96 minus sum step), the Netherlands and
///   Poland (weighted mod 11) and Italy (Luhn over the eleven digits, office code kept in the range that is
///   handed out). More countries only carry a format without a published check digit, they can be added as
///   formats become verifiable.
///   The numbers are syntactically valid, they are not registered and do not identify a real company. A VIES
///   lookup will therefore report them as not registered, which is the correct answer for test data.
/// </remarks>
public abstract class VatIdSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   The countries this source can generate, as ISO 3166-1 alpha-2 codes.
   /// </summary>
   public static IReadOnlyCollection<string> SupportedCountries { get; } = ["AT", "DE", "HR", "IT", "NL", "PL"];

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
         "DE" => NextGermanId(),
         "HR" => NextCroatianId(),
         "IT" => NextItalianId(),
         "NL" => NextDutchId(),
         "PL" => NextPolishId(),
         _ => throw new InvalidOperationException($"Country '{country}' has no VAT ID generator.")
      };
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
