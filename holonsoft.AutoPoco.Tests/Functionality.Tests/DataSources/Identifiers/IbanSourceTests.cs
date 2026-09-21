using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class IbanSourceTests : TestBase {
   /// <summary>
   ///   The official example IBAN of every SEPA country from the SWIFT registry, so the validator is
   ///   measured against the registry before it judges the source. Every example also exercises the
   ///   national check digits of its country.
   /// </summary>
   [Theory]
   [InlineData("AD1200012030200359100100")]
   [InlineData("AT611904300234573201")]
   [InlineData("BE68539007547034")]
   [InlineData("BG80BNBG96611020345678")]
   [InlineData("CH9300762011623852957")]
   [InlineData("CY17002001280000001200527600")]
   [InlineData("CZ6508000000192000145399")]
   [InlineData("DE89370400440532013000")]
   [InlineData("DK5000400440116243")]
   [InlineData("EE382200221020145685")]
   [InlineData("ES9121000418450200051332")]
   [InlineData("FI2112345600000785")]
   [InlineData("FR1420041010050500013M02606")]
   [InlineData("GB29NWBK60161331926819")]
   [InlineData("GI75NWBK000000007099453")]
   [InlineData("GR1601101250000000012300695")]
   [InlineData("HR1210010051863000160")]
   [InlineData("HU42117730161111101800000000")]
   [InlineData("IE29AIBK93115212345678")]
   [InlineData("IS140159260076545510730339")]
   [InlineData("IT60X0542811101000000123456")]
   [InlineData("LI21088100002324013AA")]
   [InlineData("LT121000011101001000")]
   [InlineData("LU280019400644750000")]
   [InlineData("LV80BANK0000435195001")]
   [InlineData("MC5811222000010123456789030")]
   [InlineData("MT84MALT011000012345MTLCAST001S")]
   [InlineData("NL91ABNA0417164300")]
   [InlineData("NO9386011117947")]
   [InlineData("PL61109010140000071219812874")]
   [InlineData("PT50000201231234567890154")]
   [InlineData("RO49AAAA1B31007593840000")]
   [InlineData("SE4550000000058398257466")]
   [InlineData("SI56263300012039086")]
   [InlineData("SK3112000000198742637541")]
   [InlineData("SM86U0322509800000000270100")]
   [InlineData("VA59001123000012345678")]
   public void TheValidatorAcceptsEveryRegistryExample(string value)
      => IdentifierValidation.IsValidIban(value).ShouldBeTrue();

   [Theory]
   [InlineData("DE89370400440532013001")]      // last digit changed, mod 97 breaks
   [InlineData("DE88370400440532013000")]      // check digits changed
   [InlineData("DE8937040044053201300")]       // one digit short
   [InlineData("DE893704004405320130000")]     // one digit too many
   [InlineData("de89370400440532013000")]      // country codes are upper case
   [InlineData("XK051212012345678906")]        // a country outside the registry
   [InlineData("NL91abna0417164300")]          // the bank code letters are upper case
   [InlineData("DEAA370400440532013000")]      // the check digits are digits
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenIban(string? value)
      => IdentifierValidation.IsValidIban(value).ShouldBeFalse();

   /// <summary>
   ///   Correct mod 97 check digits alone are not enough: a number whose national check digits are broken
   ///   has to fall through even when its mod 97 digits are recalculated to match.
   /// </summary>
   [Theory]
   [InlineData("BE", "539007547035")]                    // Belgian check is 34, not 35
   [InlineData("IT", "Y0542811101000000123456")]         // the CIN of this BBAN is X, not Y
   [InlineData("NO", "86011117948")]                     // Norwegian check is 7, not 8
   [InlineData("PL", "109010150000071219812874")]        // the bank code check digit is 4, not 5
   public void CorrectMod97DigitsDoNotRescueBrokenNationalChecks(string countryCode, string bban) {
      var check = 98 - CheckDigits.Mod97($"{bban}{countryCode}00");
      var iban = $"{countryCode}{check:00}{bban}";

      IdentifierValidation.IsValidIban(iban).ShouldBeFalse($"'{iban}' passed although its national check digits are broken");
   }

   public static TheoryData<string> AllCountries()
      => [.. IbanSourceBase.SupportedCountries];

   [Theory]
   [MemberData(nameof(AllCountries))]
   public void EveryIbanOfAFixedCountryIsValid(string countryCode) {
      var source = new IbanSource(countryCode);

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith(countryCode);
         IdentifierValidation.IsValidIban(value).ShouldBeTrue($"'{value}' does not validate");
      }
   }

   [Fact]
   public void TheRandomCountryProducesEveryCountry() {
      var source = new IbanSource();

      var countries = Enumerable.Range(0, 2000)
         .Select(_ => source.Next(null)[..2])
         .Distinct()
         .OrderBy(c => c, StringComparer.Ordinal)
         .ToList();

      countries.ShouldBe(IbanSourceBase.SupportedCountries.OrderBy(c => c, StringComparer.Ordinal).ToList());
   }

   [Fact]
   public void AFixedCountryStaysFixed()
      => Enumerable.Range(0, 100)
         .Select(_ => new IbanSource("FR").Next(null))
         .ShouldAllBe(v => v.StartsWith("FR"));

   /// <summary>
   ///   Mod 97 detects every single digit substitution, so no mutation of a German IBAN may still validate.
   /// </summary>
   [Fact]
   public void NoSingleDigitChangeOfAGeneratedIbanStillValidates() {
      var source = new IbanSource("DE");

      for (var i = 0; i < 10; i++) {
         var value = source.Next(null);

         foreach (var mutatedDigits in IdentifierValidation.SingleCharacterMutations(value[2..], false)) {
            var mutated = $"DE{mutatedDigits}";
            IdentifierValidation.IsValidIban(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
         }
      }
   }

   [Theory]
   [InlineData("XX")]
   [InlineData("de")]
   [InlineData("US")]      // not a SEPA country
   public void AnUnsupportedCountryThrows(string countryCode)
      => Should.Throw<ArgumentException>(() => new IbanSource(countryCode))
         .Message.ShouldContain(countryCode);

   [Fact]
   public void AnIbanCarriesNoSpaces()
      => Enumerable.Range(0, 200).Select(_ => new IbanSource().Next(null)).ShouldAllBe(v => !v.Contains(' '));

   [Fact]
   public void NextReturnsStableIbanListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new IbanSource(), new string[] { "NO5134313807225", "HU56809636442286500365506095", "SK8755243891483297669174", "NL39DQGO7100051863", "RO05WMVXQ7WJ4X3RRJFGCEHI", "CH1138307J4SUGDGWO0FP", "AT085815904679793127", "CH6423867A2W4VB0CN5WV", "IS945560963976785002938969", "IT55J0781761807ANPMU0BVNHPC" });

   [Fact]
   public void NextReturnsStableIbanListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableIbanSource()!, new string?[] { "NO5134313807225", "HU56809636442286500365506095", "SK8755243891483297669174", "NL39DQGO7100051863", "RO05WMVXQ7WJ4X3RRJFGCEHI", "CH1138307J4SUGDGWO0FP", "AT085815904679793127", "CH6423867A2W4VB0CN5WV", "IS945560963976785002938969", "IT55J0781761807ANPMU0BVNHPC", "SM79R1306283286F83L4YFJ5PXV", "SI56312451787313411", "DE80778656253213312999", "FI5470703677052007", "SK7642732802303039249251", "IS475613600922413090076467", "AT446718929288077842", "IS667562078514892497667634", "IT89A5823633902I6BHIWN1THES", "LV83IQYB3FFC63NO95NDK", "GR769813341HPYHTNVJ7KDMZETG", "AD9819244352X6HYF09ORXX5", "CZ9677013095297720295334", "PL38565423709134939250743971", null, "BG08OHKM703324U6T5BERF", "PT50193008023856669286742", null, "AD6117966826O53X3IGR55NW", "HR8117517622098294691" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidIbans() {
      var source = new NullableIbanSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidIban(value).ShouldBeTrue($"'{value}' does not validate");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new IbanSource().Next(null)).ShouldAllBe(v => v != null);
}

public class TestBlzIbanSourceTests : TestBase {
   [Fact]
   public void EveryNumberIsAGermanIbanOnANineBankCode() {
      var source = new TestBlzIbanSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(22);
         value.ShouldStartWith("DE");
         value[4].ShouldBe('9', $"'{value}' does not start its bank code with a nine");
         IdentifierValidation.IsValidIban(value).ShouldBeTrue($"'{value}' does not validate");
      }
   }

   /// <summary>
   ///   The plain source draws the whole bank code, this one pins its first digit outside the assigned
   ///   range. Two sources on the same seed therefore have to disagree, otherwise nothing is pinned.
   /// </summary>
   [Fact]
   public void TheNumbersDifferFromThePlainIbanSource() {
      var plain = new IbanSource("DE");
      var onTestCodes = new TestBlzIbanSource();

      var fromPlain = Enumerable.Range(0, 20).Select(_ => plain.Next(null)).ToList();
      var fromTestCodes = Enumerable.Range(0, 20).Select(_ => onTestCodes.Next(null)).ToList();

      fromTestCodes.ShouldNotBe(fromPlain);
   }

   [Fact]
   public void NextReturnsStableIbanListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new TestBlzIbanSource(), new string[] { "DE09934313807226180963", "DE93964322865003655060", "DE83992255243891483297", "DE54966917430671000518", "DE64963657670341380301", "DE87971243830731140302", "DE96960080791581590467", "DE25997931274238672048", "DE41907570355609639767", "DE25985002938979400781" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidIbans() {
      var source = new NullableTestBlzIbanSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidIban(value).ShouldBeTrue($"'{value}' does not validate");
   }
}
