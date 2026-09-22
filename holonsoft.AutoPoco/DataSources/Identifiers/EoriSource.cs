using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an EORI number, the customs identifier of an economic operator in the EU: the country code
///   and up to fifteen alphanumeric characters per Annex 12-01 of the UCC Implementing Regulation.
/// </summary>
/// <remarks>
///   The EORI format itself carries no check digit, but most countries build the national part from an
///   identifier that does, and this source builds it the same way: Belgium from an enterprise number with
///   its mod 97 tail, Denmark from a CVR number divisible by eleven, France from a SIRET whose SIREN and
///   whole number both satisfy Luhn, Croatia from an OIB, Italy from a partita IVA, Poland from a NIP with
///   five zeros behind it, the United Kingdom and Northern Ireland from a VAT registration number with 000
///   behind it, the Netherlands from nine digits of fiscal number and Germany from fifteen digits, which is
///   all their formats require. Austria is left out because no official source publishes its structure, and
///   the Portuguese third country scheme because its check digit algorithm is unpublished; a country we
///   cannot make verifiable stays out.
///   The numbers are syntactically valid, they are not registered and do not identify a real operator, so
///   the EU validation service will report them as not registered, which is the correct answer for test data.
/// </remarks>
public abstract class EoriSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   The countries this source can generate, as the prefixes the numbers carry.
   /// </summary>
   public static IReadOnlyCollection<string> SupportedCountries { get; } =
      ["BE", "DE", "DK", "FR", "GB", "HR", "IT", "NL", "PL", "XI"];

   private static readonly string[] _countries = [.. SupportedCountries];

   private readonly string? _fixedCountry;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="countryCode">
   ///   Optional country prefix, e.g. <c>DE</c>. Drawn from <see cref="SupportedCountries" /> when null.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="countryCode" /> is not a supported country.</exception>
   protected EoriSourceBase(string? countryCode = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (countryCode is not null) {
         if (!SupportedCountries.Contains(countryCode))
            throw new ArgumentException(
               $"'{countryCode}' is not a country this source can generate an EORI number for. Supported: {string.Join(", ", SupportedCountries)}.",
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
         "BE" => $"BE{NextBelgianEnterpriseNumber()}",
         "DE" => $"DE{NextDigits(15)}",
         "DK" => $"DK{NextDanishCvrNumber()}",
         "FR" => $"FR{NextFrenchSiret()}",
         "GB" or "XI" => $"{country}{NextUnitedKingdomVatRegistrationNumber()}000",
         "HR" => $"HR{NextCroatianOib()}",
         "IT" => $"IT{NextItalianPartitaIva()}",
         "NL" => $"NL{NextDigits(9)}",
         "PL" => $"PL{NextPolishNip()}00000",
         _ => throw new InvalidOperationException($"Country '{country}' has no EORI generator.")
      };
   }

   private string NextDigits(int count) {
      Span<char> digits = stackalloc char[count];

      for (var i = 0; i < count; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      return new string(digits);
   }

   /// <summary>
   ///   Ten digits: a leading zero or one, a second digit off zero, and the pair 97 minus the first eight
   ///   mod 97, the same arithmetic the Belgian VAT ID uses.
   /// </summary>
   private string NextBelgianEnterpriseNumber() {
      Span<char> digits = stackalloc char[10];
      digits[0] = (char) ('0' + Random.Next(0, 2));
      digits[1] = (char) ('1' + Random.Next(0, 9));

      var body = digits[0] - '0';
      body = (body * 10) + (digits[1] - '0');

      for (var i = 2; i < 8; i++) {
         digits[i] = (char) ('0' + Random.Next(0, 10));
         body = (body * 10) + (digits[i] - '0');
      }

      var check = 97 - (body % 97);
      digits[8] = (char) ('0' + (check / 10));
      digits[9] = (char) ('0' + (check % 10));

      return new string(digits);
   }

   /// <summary>
   ///   Eight digits, the first off zero, weighted 2, 7, 6, 5, 4, 3, 2, 1 divisible by eleven; drawn again
   ///   when no last digit achieves that.
   /// </summary>
   private string NextDanishCvrNumber() {
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

      return new string(digits);
   }

   /// <summary>
   ///   Fourteen digits: eight digits and their Luhn check digit form the SIREN, four digits of
   ///   establishment number follow, and the last digit makes the whole SIRET pass the Luhn check too.
   /// </summary>
   private string NextFrenchSiret() {
      Span<char> digits = stackalloc char[14];

      for (var i = 0; i < 8; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[8] = (char) ('0' + CheckDigits.Luhn(digits, 8));

      for (var i = 9; i < 13; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[13] = (char) ('0' + CheckDigits.Luhn(digits, 13));

      return new string(digits);
   }

   /// <summary>
   ///   Nine digits of VAT registration number: seven digits weighted 8 down to 2, closed by the two digit
   ///   remainder of the MOD 97 or the MOD 9755 scheme, each with its excluded leading ranges.
   /// </summary>
   private string NextUnitedKingdomVatRegistrationNumber() {
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

      return new string(digits);
   }

   /// <summary>
   ///   The eleven digit OIB with its ISO 7064 MOD 11,10 check digit.
   /// </summary>
   private string NextCroatianOib() {
      Span<char> digits = stackalloc char[11];

      for (var i = 0; i < 10; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[10] = (char) ('0' + CheckDigits.Iso7064Mod1110(digits, 10));

      return new string(digits);
   }

   /// <summary>
   ///   The eleven digit partita IVA: seven digits, an office code between 001 and 100 and the Luhn check digit.
   /// </summary>
   private string NextItalianPartitaIva() {
      Span<char> digits = stackalloc char[11];

      for (var i = 0; i < 7; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var office = 1 + Random.Next(0, 100);
      digits[7] = (char) ('0' + (office / 100));
      digits[8] = (char) ('0' + (office / 10 % 10));
      digits[9] = (char) ('0' + (office % 10));

      digits[10] = (char) ('0' + CheckDigits.Luhn(digits, 10));

      return new string(digits);
   }

   /// <summary>
   ///   The ten digit NIP, the first digit off zero, closed by its weighted mod 11 check digit; drawn again
   ///   when the weighted sum leaves remainder ten.
   /// </summary>
   private string NextPolishNip() {
      ReadOnlySpan<int> weights = [6, 5, 7, 2, 3, 4, 5, 6, 7];
      Span<char> digits = stackalloc char[10];
      int check;

      do {
         digits[0] = (char) ('1' + Random.Next(0, 9));

         for (var i = 1; i < 9; i++)
            digits[i] = (char) ('0' + Random.Next(0, 10));

         check = CheckDigits.WeightedMod11(digits, weights);
      } while (check == 10);

      digits[9] = (char) ('0' + check);

      return new string(digits);
   }
}

/// <summary>
///   An EORI number whose national part carries the check digits of the underlying national identifier.
/// </summary>
public class EoriSource(string? countryCode) : EoriSourceBase(countryCode) {
   public EoriSource() : this(null) { }
}

/// <summary>
///   An EORI number that returns null every now and then.
/// </summary>
public class NullableEoriSource(string? countryCode, int nullCreationThreshold) : EoriSourceBase(countryCode, nullCreationThreshold) {
   public NullableEoriSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableEoriSource(string? countryCode) : this(countryCode, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableEoriSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
