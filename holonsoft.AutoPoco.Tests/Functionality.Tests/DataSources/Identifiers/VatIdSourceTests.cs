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
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenVatId(string? value)
      => IdentifierValidation.IsValidVatId(value).ShouldBeFalse();

   [Theory]
   [InlineData("AT", 11)]
   [InlineData("DE", 11)]
   [InlineData("HR", 13)]
   [InlineData("IT", 13)]
   [InlineData("NL", 14)]
   [InlineData("PL", 12)]
   public void EveryNumberOfAFixedCountryIsValid(string countryCode, int expectedLength) {
      var source = new VatIdSource(countryCode);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(expectedLength);
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
   ///   Every check digit scheme in here detects any single digit substitution, so no mutation of the
   ///   checksummed digits may still validate. The Dutch suffix carries no check digit and is left alone.
   /// </summary>
   [Theory]
   [InlineData("AT")]
   [InlineData("DE")]
   [InlineData("HR")]
   [InlineData("IT")]
   [InlineData("NL")]
   [InlineData("PL")]
   public void NoSingleDigitChangeOfTheChecksummedPartStillValidates(string countryCode) {
      var source = new VatIdSource(countryCode);

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         var digitsStart = countryCode == "AT" ? 3 : 2;
         var digitsEnd = countryCode == "NL" ? 11 : value.Length;
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
         new VatIdSource(), new string[] { "NL343138074B93", "HR26180963647", "IT22865000685", "PL7550609224", "PL6243891485", "IT29766910789", "HR43067100051", "DE963657675", "ATU34138037", "HR01712438307" });

   [Fact]
   public void NextReturnsStableVatIdListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableVatIdSource()!, new string?[] { "NL343138074B93", "HR26180963647", "IT22865000685", "PL7550609224", "PL6243891485", "IT29766910789", "HR43067100051", "DE963657675", "ATU34138037", "HR01712438307", "NL731140308B64", "HR60080791588", "NL159046798B24", "DE412742380", "IT67204800139", "PL8035560965", "IT97678500642", "ATU29389792", "NL007817617B41", "ATU72797967", "PL9609770190", "DE401306280", "NL328678995B68", "PL9542138596", null, "DE231245175", "PL9731344115", null, "PL4213312990", "DE970703677" });

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
