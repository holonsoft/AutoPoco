using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class VinSourceTests : TestBase {
   /// <summary>
   ///   Known valid numbers, so the validator is measured against the world before it judges the source:
   ///   the standard's own example whose check digit is the X for ten, and the all ones number whose
   ///   weighted sum is 89, which is 1 mod 11.
   /// </summary>
   [Theory]
   [InlineData("1M8GDM9AXKP042788")]
   [InlineData("11111111111111111")]
   public void TheValidatorAcceptsAKnownValidVin(string value)
      => IdentifierValidation.IsValidVin(value).ShouldBeTrue();

   [Theory]
   [InlineData("1M8GDM9A0KP042788")]     // check digit 0 where the X belongs
   [InlineData("1M8GDM9A1KP042788")]     // check digit 1 where the X belongs
   [InlineData("2M8GDM9AXKP042788")]     // a data character changed, the X no longer fits
   [InlineData("1M8GDM9AXKP04278")]      // one character short
   [InlineData("1M8GDM9AXKP0427888")]    // one character too many
   [InlineData("IM8GDM9AXKP042788")]     // an I never appears in a VIN
   [InlineData("1m8gdm9axkp042788")]     // capital letters only
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenVin(string? value)
      => IdentifierValidation.IsValidVin(value).ShouldBeFalse();

   [Fact]
   public void EveryVinHasSeventeenCharactersAndAValidCheckDigit() {
      var source = new VinSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(17);
         IdentifierValidation.IsValidVin(value).ShouldBeTrue($"'{value}' has a broken check digit");
         value.ShouldAllBe(c => c != 'I' && c != 'O' && c != 'Q');
         "UZ0".ShouldNotContain(value[9], $"'{value}' carries a model year character 49 CFR 565 does not allow");
      }
   }

   /// <summary>
   ///   The check digit ten is written as an X and comes up in roughly one of eleven numbers, so a sample
   ///   of a few hundred has to contain one.
   /// </summary>
   [Fact]
   public void ACheckDigitOfTenIsWrittenAsAnX() {
      var source = new VinSource();

      Enumerable.Range(0, 500)
         .Select(_ => source.Next(null))
         .ShouldContain(v => v[8] == 'X');
   }

   [Fact]
   public void APrefixPinsTheWorldManufacturerIdentifier() {
      var source = new VinSource("WVW");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith("WVW");
         IdentifierValidation.IsValidVin(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Theory]
   [InlineData("WVI")]           // I is not a VIN character
   [InlineData("wvw")]           // capital letters only
   [InlineData("WVWZZZ1JZ")]     // nine characters reach into the check digit
   public void ABrokenPrefixThrows(string prefix)
      => Should.Throw<ArgumentException>(() => new VinSource(prefix));

   [Fact]
   public void NextReturnsStableVinListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new VinSource(), new string[] { "V3LKZPV203H63ULTT", "TJ8X5ZG32Y65MAZWN", "G9YJ5M4Y6SS1C4V3A", "S6UN9HDT5MU3G6TP1", "ZGYVN3YC0NT7ZK43U", "UKFGCEHJ5Y4X3Y3VK", "4VXGDGZR81FS1M8CV", "ZBT679PW2L14KN7A2", "Z4YB0CP548ZYK5DN0", "PP8MZV3S4DPLGGHPC" });

   [Fact]
   public void NextReturnsStableVinListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableVinSource()!, new string?[] { "V3LKZPV203H63ULTT", "TJ8X5ZG32Y65MAZWN", "G9YJ5M4Y6SS1C4V3A", "S6UN9HDT5MU3G6TP1", "ZGYVN3YC0NT7ZK43U", "UKFGCEHJ5Y4X3Y3VK", "4VXGDGZR81FS1M8CV", "ZBT679PW2L14KN7A2", "Z4YB0CP548ZYK5DN0", "PP8MZV3S4DPLGGHPC", "PAPSNX0B18YPHSC0V", "DKGDCB2V6YBF83M4F", "K5SYW1JL76V1WR7XV", "F3134YXE0M17F56JE", "VUEH2X9B980GK7VTG", "X5F0ZAVZ33L22VWZJ", "3X0KXLK154G0XSC23", "DGD7Y67P226VU8XV2", "9XRGP84K0NBH936CD", "HYRV41KL7XGJPM2Z8", "Y5HBR9Y2X5FSBWP6N", "U0LFM8KK7LZEJ6BHJ", "ZP1WHEVR39G3FFC63", "PR95PDLF4A1VKLVAH", null, "SHWPYK7L1WDNEWG09", "DJ44WK269XHF09RU5", null, "6PZHD30563YYBPV0E", "A5WWW5MX3MCTJ7SBS" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableVinSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidVin(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new VinSource().Next(null)).ShouldAllBe(v => v != null);
}
