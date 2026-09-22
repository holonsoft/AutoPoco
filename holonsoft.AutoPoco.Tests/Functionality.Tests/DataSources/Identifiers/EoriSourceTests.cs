using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class EoriSourceTests : TestBase {
   /// <summary>
   ///   EORI numbers built from registered national identifiers of real operators (an EORI's national part
   ///   is the national identifier), plus the German shape the chambers of commerce publish, so the
   ///   validator is measured against the world before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("BE0776091951")]         // the enterprise number from the BMF worked example
   [InlineData("DE000000001234567")]    // the zero padded shape from the IHK documentation
   [InlineData("DK26259495")]           // Coop Danmark's CVR number
   [InlineData("GB434031494000")]       // a checksummed VAT registration number plus 000
   [InlineData("XI434031494000")]       // Northern Ireland runs on the same scheme
   [InlineData("HR29524210204")]        // a registered OIB
   [InlineData("IT00000010215")]        // a registered partita IVA
   [InlineData("NL002065538")]          // Philips' fiscal number
   [InlineData("PL521220725700000")]    // a registered NIP plus the five zeros
   public void TheValidatorAcceptsAWellFormedEori(string value)
      => IdentifierValidation.IsValidEori(value).ShouldBeTrue();

   /// <summary>
   ///   The examples the authorities publish are format illustrations, not checksummed numbers: the HMRC
   ///   example fails the VAT registration check and the EC example fails the CVR check. The validator is
   ///   stricter than the format on purpose, because the source only generates checksummed national parts.
   /// </summary>
   [Theory]
   [InlineData("GB012345678000")]       // the HMRC form example
   [InlineData("DK11223344")]           // the EC national implementation example
   public void TheOfficialFormatIllustrationsFailTheNationalChecks(string value)
      => IdentifierValidation.IsValidEori(value).ShouldBeFalse();

   [Theory]
   [InlineData("BE0776091952")]         // enterprise number check pair off by one
   [InlineData("DE00000000123456")]     // fourteen digits instead of fifteen
   [InlineData("DE0000000012345678")]   // sixteen digits instead of fifteen
   [InlineData("DEA00000001234567")]    // Germany uses digits only
   [InlineData("DK26259496")]           // CVR no longer divisible by eleven
   [InlineData("FR00000000000001")]     // the SIRET no longer passes the Luhn check
   [InlineData("FR10000000000000")]     // the SIREN no longer passes the Luhn check
   [InlineData("GB434031494")]          // the 000 tail is missing
   [InlineData("GB434031494001")]       // the tail is exactly three zeros
   [InlineData("HR29524210205")]        // OIB check digit off by one
   [InlineData("IT00000010216")]        // partita IVA check digit off by one
   [InlineData("NL00206553")]           // eight digits instead of nine
   [InlineData("PL521220725712345")]    // the tail behind the NIP is five zeros
   [InlineData("PL521220725800000")]    // NIP check digit off by one
   [InlineData("AT1234567890")]         // Austria publishes no structure, so it is not generated or accepted
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenEori(string? value)
      => IdentifierValidation.IsValidEori(value).ShouldBeFalse();

   [Theory]
   [InlineData("BE", 12)]
   [InlineData("DE", 17)]
   [InlineData("DK", 10)]
   [InlineData("FR", 16)]
   [InlineData("GB", 14)]
   [InlineData("HR", 13)]
   [InlineData("IT", 13)]
   [InlineData("NL", 11)]
   [InlineData("PL", 17)]
   [InlineData("XI", 14)]
   public void EveryNumberOfAFixedCountryIsValid(string countryCode, int expectedLength) {
      var source = new EoriSource(countryCode);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(expectedLength);
         value.ShouldStartWith(countryCode);
         IdentifierValidation.IsValidEori(value).ShouldBeTrue($"'{value}' does not validate");
      }
   }

   [Fact]
   public void TheRandomCountryProducesEveryCountry() {
      var source = new EoriSource();

      var countries = Enumerable.Range(0, 500)
         .Select(_ => source.Next(null)[..2])
         .Distinct()
         .OrderBy(c => c, StringComparer.Ordinal)
         .ToList();

      countries.ShouldBe(EoriSourceBase.SupportedCountries.OrderBy(c => c, StringComparer.Ordinal).ToList());
   }

   [Fact]
   public void AFixedCountryStaysFixed()
      => Enumerable.Range(0, 100)
         .Select(_ => new EoriSource("FR").Next(null))
         .ShouldAllBe(v => v.StartsWith("FR"));

   [Theory]
   [InlineData("AT")]      // structure not published, deliberately unsupported
   [InlineData("PT")]      // third country check digit unpublished, deliberately unsupported
   [InlineData("de")]
   [InlineData("XX")]
   public void AnUnsupportedCountryThrows(string countryCode)
      => Should.Throw<ArgumentException>(() => new EoriSource(countryCode))
         .Message.ShouldContain(countryCode);

   [Fact]
   public void NextReturnsStableEoriListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new EoriSource(), new string[] { "FR43138072426189", "BE1736432236", "PL750036550100000", "IT09225520692", "FR89148329976691", "DE743067100051863", "IT57670340348", "FR80301712843833", "BE0831140342", "BE1360080748" });

   [Fact]
   public void NextReturnsStableEoriListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableEoriSource()!, new string?[] { "FR43138072426189", "BE1736432236", "PL750036550100000", "IT09225520692", "FR89148329976691", "DE743067100051863", "IT57670340348", "FR80301712843833", "BE0831140342", "BE1360080748", "XI107930424000", "DE590467979312742", "FR86720480275700", "FR55609639476789", "HR00293897945", "BE0881761870", "BE1379796888", "IT09770190909", "FR01306283128676", "PL835854213600000", "PL691913124100000", "HR17873134413", "DE577786562532133", "DE299998707036770", null, "HR20002427320", "PL123030392800000", null, "GB706973062000", "HR13561360091" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableEoriSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidEori(value).ShouldBeTrue($"'{value}' does not validate");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new EoriSource().Next(null)).ShouldAllBe(v => v != null);
}
