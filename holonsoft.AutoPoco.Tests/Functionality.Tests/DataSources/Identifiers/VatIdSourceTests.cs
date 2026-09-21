using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class VatIdSourceTests : TestBase {
   /// <summary>
   ///   Registered VAT IDs of real companies and the worked example of the Austrian BMF specification, so the
   ///   validator is measured against the world before it judges the source. A VAT ID is public data, it is
   ///   printed on every invoice.
   /// </summary>
   [Theory]
   [InlineData("DE249051813")]      // BZH Vogt
   [InlineData("DE198612614")]      // Sausalitos München
   [InlineData("DE129430206")]
   [InlineData("DE815243055")]
   [InlineData("ATU13585627")]      // the worked example of the BMF check digit specification
   [InlineData("ATU12345675")]      // digits 1234567: sum 31, check (96 - 31) mod 10 = 5
   [InlineData("NL002065538B01")]   // Philips
   [InlineData("IT00000010215")]
   [InlineData("PL1130027802")]
   [InlineData("PL5212207257")]
   [InlineData("PL5850010588")]
   [InlineData("HR29524210204")]
   [InlineData("BE0776091951")]      // the worked example of the BMF construction rules
   [InlineData("DK88146328")]        // BMF worked example
   [InlineData("DK26259495")]        // Coop Danmark
   [InlineData("FI09853608")]        // BMF worked example
   [InlineData("GB434031494")]       // BMF worked example, the MOD 97 branch
   [InlineData("XI434031494")]       // Northern Ireland runs on the same scheme
   [InlineData("IE3628739L")]        // BMF worked example, eight characters
   [InlineData("IE3628739UA")]       // BMF worked example, nine characters
   [InlineData("LU10000356")]        // BMF worked example
   [InlineData("PT502757191")]       // BMF worked example
   [InlineData("SE556188840401")]    // BMF worked example
   [InlineData("EE100207415")]       // BMF worked example
   [InlineData("HU21376414")]        // BMF worked example
   [InlineData("HU10597190")]        // BMF worked example, check digit zero
   [InlineData("LT213179412")]       // BMF worked example, decided by the second weight pass
   [InlineData("LT290061371314")]    // BMF worked example of the twelve digit form
   [InlineData("SI15012557")]        // BMF worked example
   [InlineData("SK4030000007")]      // BMF worked example
   public void TheValidatorAcceptsRealVatIds(string value)
      => IdentifierValidation.IsValidVatId(value).ShouldBeTrue();

   [Theory]
   [InlineData("DE249051814")]      // check digit off by one
   [InlineData("DE049051813")]      // a German ID never starts with a zero
   [InlineData("ATU12345679")]      // the check digit a widespread wrong formula produces for 1234567
   [InlineData("ATA12345675")]      // the letter has to be a U
   [InlineData("NL002065538B00")]   // the suffix starts at 01
   [InlineData("NL002065539B01")]   // check digit relevant digit off by one
   [InlineData("IT00000010216")]    // Luhn check digit off by one
   [InlineData("IT00000009993")]    // the Luhn check digit is correct, but office code 999 is not handed out
   [InlineData("PL5212207258")]     // check digit off by one
   [InlineData("PL0212207257")]     // a NIP never starts with a zero
   [InlineData("HR29524210205")]    // check digit off by one
   [InlineData("FR12345678901")]    // a country the source does not generate
   [InlineData("DE24905181")]       // one digit short
   [InlineData("DE2490518131")]     // one digit too many
   [InlineData("BE0776091952")]     // check pair off by one
   [InlineData("BE2776091951")]     // the first digit is a zero or a one
   [InlineData("DK88146327")]       // no longer divisible by eleven
   [InlineData("FI09853609")]       // check digit off by one
   [InlineData("GB434031493")]      // neither scheme lands on a multiple of 97
   [InlineData("GB050000062")]      // the sum fits MOD 97, but 0500000 lies in a range that scheme excludes
   [InlineData("IE3628739M")]       // check letter off by one
   [InlineData("IE3628739UB")]      // the trailing letter changes the sum, so the check letter no longer fits
   [InlineData("LU10000357")]       // check pair off by one
   [InlineData("PT502757192")]      // check digit off by one
   [InlineData("SE556188840501")]   // the Luhn digit of the organisation number is broken
   [InlineData("SE556188840400")]   // the suffix starts at 01
   [InlineData("SE556188840495")]   // the suffix ends at 94
   [InlineData("EE100207416")]      // check digit off by one
   [InlineData("EE200207415")]      // an Estonian number starts with 10
   [InlineData("HU21376415")]       // check digit off by one
   [InlineData("LT213179413")]      // check digit off by one
   [InlineData("LT213179402")]      // the eighth digit is always a one
   [InlineData("SI15012558")]       // check digit off by one
   [InlineData("SI05012557")]       // a Slovenian number has no leading zero
   [InlineData("SK4030000008")]     // no longer divisible by eleven
   [InlineData("SK5407062531")]     // the counter example of the BMF document: third digit invalid, not divisible
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenVatId(string? value)
      => IdentifierValidation.IsValidVatId(value).ShouldBeFalse();

   [Theory]
   [InlineData("AT", 11, 11)]
   [InlineData("BE", 12, 12)]
   [InlineData("DE", 11, 11)]
   [InlineData("DK", 10, 10)]
   [InlineData("EE", 11, 11)]
   [InlineData("FI", 10, 10)]
   [InlineData("GB", 11, 11)]
   [InlineData("HR", 13, 13)]
   [InlineData("HU", 10, 10)]
   [InlineData("IE", 10, 11)]      // eight or nine characters behind the prefix
   [InlineData("IT", 13, 13)]
   [InlineData("LT", 11, 11)]
   [InlineData("LU", 10, 10)]
   [InlineData("NL", 14, 14)]
   [InlineData("PL", 12, 12)]
   [InlineData("PT", 11, 11)]
   [InlineData("SE", 14, 14)]
   [InlineData("SI", 10, 10)]
   [InlineData("SK", 12, 12)]
   [InlineData("XI", 11, 11)]
   public void EveryNumberOfAFixedCountryIsValid(string countryCode, int minLength, int maxLength) {
      var source = new VatIdSource(countryCode);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         value.Length.ShouldBeInRange(minLength, maxLength);
         value.ShouldStartWith(countryCode);
         IdentifierValidation.IsValidVatId(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void TheRandomCountryProducesEveryCountry() {
      var source = new VatIdSource();

      var countries = Enumerable.Range(0, 500)
         .Select(_ => source.Next(null)[..2])
         .Distinct()
         .OrderBy(c => c, StringComparer.Ordinal)
         .ToList();

      countries.ShouldBe(VatIdSourceBase.SupportedCountries.OrderBy(c => c, StringComparer.Ordinal).ToList());
   }

   [Fact]
   public void AFixedCountryStaysFixed()
      => Enumerable.Range(0, 100)
         .Select(_ => new VatIdSource("NL").Next(null))
         .ShouldAllBe(v => v.StartsWith("NL"));

   /// <summary>
   ///   These check digit schemes detect any single digit substitution, so no mutation of the checksummed
   ///   digits may still validate. The Dutch suffix and the Irish and Swedish tails carry no check digit
   ///   and are left alone. GB and XI stay out because a mutation can legitimately flip a number from the
   ///   MOD 97 scheme into the MOD 9755 scheme, LT because its second weight pass can rescue a mutation,
   ///   and PT because its check digit folds ten and eleven onto the same zero.
   /// </summary>
   [Theory]
   [InlineData("AT")]
   [InlineData("BE")]
   [InlineData("DE")]
   [InlineData("DK")]
   [InlineData("EE")]
   [InlineData("FI")]
   [InlineData("HR")]
   [InlineData("HU")]
   [InlineData("IE")]
   [InlineData("IT")]
   [InlineData("LU")]
   [InlineData("NL")]
   [InlineData("PL")]
   [InlineData("SE")]
   [InlineData("SI")]
   [InlineData("SK")]
   public void NoSingleDigitChangeOfTheChecksummedPartStillValidates(string countryCode) {
      var source = new VatIdSource(countryCode);

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         var digitsStart = countryCode == "AT" ? 3 : 2;
         var digitsEnd = countryCode switch {
            "NL" => 11,                    // the B and the suffix carry no check digit
            "IE" => 9,                     // the check letter and the trailing letter are not digits
            "SE" => value.Length - 2,      // the suffix carries no check digit
            _ => value.Length
         };
         var digits = value[digitsStart..digitsEnd];

         foreach (var mutatedDigits in IdentifierValidation.SingleCharacterMutations(digits, false)) {
            var mutated = value[..digitsStart] + mutatedDigits + value[digitsEnd..];
            IdentifierValidation.IsValidVatId(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
         }
      }
   }

   [Theory]
   [InlineData("XX")]
   [InlineData("de")]      // country codes are upper case
   [InlineData("FR")]      // real country, but its check digit is not implemented
   [InlineData("")]
   public void AnUnsupportedCountryThrows(string countryCode)
      => Should.Throw<ArgumentException>(() => new VatIdSource(countryCode))
         .Message.ShouldContain(countryCode.Length == 0 ? "Supported" : countryCode);

   [Fact]
   public void NextReturnsStableVatIdListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new VatIdSource(), new string[] { "PT443138079", "DE361809634", "PL7432286509", "SE365506092370", "LU52438901", "BE0583297622", "IE1743067M", "BE0100518625", "DK51380304", "LU17124307" });

   [Fact]
   public void NextReturnsStableVatIdListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableVatIdSource()!, new string?[] { "PT443138079", "DE361809634", "PL7432286509", "SE365506092370", "LU52438901", "BE0583297622", "IE1743067M", "BE0100518625", "DK51380304", "LU17124307", "HU40731142", "LU03026000", "ATU80791582", "LU15904603", "HR97931274232", "HU77204800", "LU75703501", "FI60963976", "GB483911274", "LT002938910", "LU79400738", "HU27618071", "LU27979669", "PL9609770190", null, "LU30130641", "LU28328688", null, "PT889973580", "EE102138595" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableVatIdSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidVatId(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new VatIdSource().Next(null)).ShouldAllBe(v => v != null);
}
