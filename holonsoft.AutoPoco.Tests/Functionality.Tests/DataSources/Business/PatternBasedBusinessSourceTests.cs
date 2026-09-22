using System.Globalization;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class SkuSourceTests : TestBase {
   [Fact]
   public void TheDefaultSkuIsThreeLettersAHyphenAndFiveDigits() {
      var source = new SkuSource();

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         Regex.IsMatch(value, @"^[A-Z]{3}-\d{5}$").ShouldBeTrue($"'{value}' does not look like the default SKU");
      }
   }

   [Fact]
   public void AnOwnPatternReplacesTheDefault() {
      var source = new SkuSource("##-@@@@-#");

      for (var i = 0; i < 100; i++)
         Regex.IsMatch(source.Next(null), @"^\d{2}-[A-Z]{4}-\d$").ShouldBeTrue();
   }

   [Fact]
   public void NextReturnsStableSkuListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new SkuSource(), new string[] { "PDU-31380", "XCC-61809", "GDO-64322", "IOW-50036", "FVK-06092", "CFM-52438", "ZBM-48329", "XGW-91743", "QGO-71000", "FBY-63657" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseSkus() {
      var source = new NullableSkuSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         Regex.IsMatch(value!, @"^[A-Z]{3}-\d{5}$").ShouldBeTrue($"'{value}' does not look like the default SKU");
   }
}

public class SerialNumberSourceTests : TestBase {
   [Fact]
   public void TheDefaultSerialNumberIsFourBlocksOfFourCharacters() {
      var source = new SerialNumberSource();

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         Regex.IsMatch(value, @"^[0-9A-Z]{4}(-[0-9A-Z]{4}){3}$").ShouldBeTrue($"'{value}' does not look like the default serial number");
      }
   }

   [Fact]
   public void AnOwnPatternReplacesTheDefault() {
      var source = new SerialNumberSource(@"SN\#@@######");

      for (var i = 0; i < 100; i++)
         Regex.IsMatch(source.Next(null), @"^SN#[A-Z]{2}\d{6}$").ShouldBeTrue();
   }

   [Fact]
   public void NextReturnsStableSerialNumberListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new SerialNumberSource(), new string[] { "S3KJ-WNS2-YH63-RKZQ", "QQI8-U5WG-3T65-LAWT", "MG9V-IY5L-4ZVP-1C4S", "3AP6-RM9H-DQR3-G6QN", "1WGX-VSM3-VCLQ-7WJ4", "X3RR-JFGC-EHI4-U3V3", "SJ4S-UGDG-WO0F-P1L8", "CXSW-BQ67-9NTJ-14YJ", "M7A2-W4VB-0CN5-WVJ5", "DM0N-N8LW-S3PN-KGGH" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseSerialNumbers() {
      var source = new NullableSerialNumberSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         Regex.IsMatch(value!, @"^[0-9A-Z]{4}(-[0-9A-Z]{4}){3}$").ShouldBeTrue($"'{value}' does not look like the default serial number");
   }
}

public class LotNumberSourceTests : TestBase {
   [Fact]
   public void TheDefaultLotNumberCarriesADateCodeInsideTheDefaultRange() {
      var source = new LotNumberSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         Regex.IsMatch(value, @"^L\d{5}-\d{4}$").ShouldBeTrue($"'{value}' does not look like the default lot number");

         var year = int.Parse(value.AsSpan(1, 2), CultureInfo.InvariantCulture);
         var dayOfYear = int.Parse(value.AsSpan(3, 3), CultureInfo.InvariantCulture);

         year.ShouldBeInRange(20, 35);
         dayOfYear.ShouldBeInRange(1, 366);
      }
   }

   [Fact]
   public void APinnedDateIsEncodedExactly() {
      var date = new DateOnly(2026, 9, 22);      // the 265th day of 2026
      var source = new LotNumberSource(date, date);

      for (var i = 0; i < 50; i++)
         source.Next(null).ShouldStartWith("L26265-");
   }

   [Fact]
   public void AnOwnSuffixPatternReplacesTheDefault() {
      var source = new LotNumberSource("@@##");

      for (var i = 0; i < 100; i++)
         Regex.IsMatch(source.Next(null), @"^L\d{5}-[A-Z]{2}\d{2}$").ShouldBeTrue();
   }

   [Fact]
   public void ReversedDatesThrow()
      => Should.Throw<ArgumentOutOfRangeException>(() => new LotNumberSource(new DateOnly(2030, 1, 1), new DateOnly(2020, 1, 1)));

   [Fact]
   public void NextReturnsStableLotNumberListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new LotNumberSource(), new string[] { "L30152-3431", "L34346-8072", "L26157-6180", "L29166-9636", "L24224-3228", "L33298-5003", "L30153-6550", "L30281-6092", "L30286-5524", "L33147-8914" });

   [Fact]
   public void NextReturnsStableLotNumberListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableLotNumberSource()!, new string?[] { "L30152-3431", "L34346-8072", "L26157-6180", "L29166-9636", "L24224-3228", "L33298-5003", "L30153-6550", "L30281-6092", "L30286-5524", "L33147-8914", "L25122-8329", "L31331-7669", "L21237-4306", "L31125-7100", "L26107-0518", "L29319-3657", "L20183-7034", "L33277-3803", "L26128-0171", "L21336-4383", "L21067-7311", "L28342-0302", "L22156-0080", "L27195-7915", null, "L28355-1590", "L32198-6797", null, "L23293-3127", "L26309-2386" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseLotNumbers() {
      var source = new NullableLotNumberSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         Regex.IsMatch(value!, @"^L\d{5}-\d{4}$").ShouldBeTrue($"'{value}' does not look like the default lot number");
   }
}
