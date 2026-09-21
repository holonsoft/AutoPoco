using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class ImeiSourceTests : TestBase {
   /// <summary>
   ///   Published IMEIs, so the Luhn check is measured against the world before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("490154203237518")]
   [InlineData("356938035643809")]
   public void TheValidatorAcceptsAPublishedImei(string value)
      => IdentifierValidation.IsValidLuhn(value).ShouldBeTrue();

   [Theory]
   [InlineData("490154203237519")]   // check digit off by one
   [InlineData("490154203237618")]   // a data digit off by one
   [InlineData("49015420323751X")]
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenImei(string? value)
      => IdentifierValidation.IsValidLuhn(value).ShouldBeFalse();

   [Fact]
   public void EveryImeiHasFifteenDigitsAndAValidCheckDigit() {
      var source = new ImeiSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(15);
         value.ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidLuhn(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void ATypeAllocationCodeStartsEveryNumber() {
      var source = new ImeiSource("35693803");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith("35693803");
         IdentifierValidation.IsValidLuhn(value).ShouldBeTrue();
      }
   }

   [Theory]
   [InlineData("3569380a")]
   [InlineData("356938035643809")]
   public void AnImpossibleTypeAllocationCodeIsRejected(string tac) {
      var ex = Should.Throw<ArgumentException>(() => new ImeiSource(tac));
      ex.ParamName.ShouldBe("typeAllocationCode");
   }

   [Fact]
   public void NoSingleDigitChangeOfAGeneratedNumberStillValidates() {
      var source = new ImeiSource();

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, false))
            IdentifierValidation.IsValidLuhn(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   [Fact]
   public void EveryCheckDigitFromZeroToNineOccurs() {
      var source = new ImeiSource();

      Enumerable.Range(0, 2000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ShouldBe("0123456789".ToCharArray());
   }

   [Fact]
   public void NextReturnsStableImeiListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new ImeiSource(), new string[] { "343138072261807", "963643228650034", "655060922552437", "891483297669178", "430671000518637", "657670341380307", "171243830731146", "030260080791589", "159046797931278", "423867204807573" });

   [Fact]
   public void NextReturnsStableImeiListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableImeiSource()!, new string?[] { "343138072261807", "963643228650034", "655060922552437", "891483297669178", "430671000518637", "657670341380307", "171243830731146", "030260080791589", "159046797931278", "423867204807573", "035560963976784", "500293897940074", "817618072797964", "860977019930138", "062832867899735", "585421385919130", "124517873134419", "157778656253210", "331299998707036", "677052000242738", "280230303924926", "513561360092242", "130900764771675", "189292880778424", null, "357194346145824", "841374027756206", null, "785148924976671", "604458236339024" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableImeiSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidLuhn(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new ImeiSource().Next(null)).ShouldAllBe(v => v != null);
}

public class IsinSourceTests : TestBase {
   /// <summary>
   ///   Real securities, so the validator is measured against the world before it judges the source. These
   ///   also prove the letter expansion, because the two prefix letters take part in the check digit.
   /// </summary>
   [Theory]
   [InlineData("US0378331005")]   // Apple
   [InlineData("GB0002634946")]   // BAE Systems
   [InlineData("DE0005557508")]   // Deutsche Telekom
   [InlineData("US5949181045")]   // Microsoft
   public void TheValidatorAcceptsARealIsin(string value)
      => IdentifierValidation.IsValidIsin(value).ShouldBeTrue();

   [Theory]
   [InlineData("US0378331006")]   // check digit off by one
   [InlineData("US0378331105")]   // a data character off by one
   [InlineData("XS0378331005")]   // the prefix takes part in the check digit, so swapping it breaks the number
   [InlineData("US037833100")]    // one character short
   [InlineData("0S0378331005")]   // the prefix has to be letters
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenIsin(string? value)
      => IdentifierValidation.IsValidIsin(value).ShouldBeFalse();

   [Fact]
   public void EveryIsinHasTwelveCharactersAndAValidCheckDigit() {
      var source = new IsinSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(12);
         value[0].ShouldBeInRange('A', 'Z');
         value[1].ShouldBeInRange('A', 'Z');
         value[^1].ShouldBeInRange('0', '9');
         IdentifierValidation.IsValidIsin(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void APrefixCanBeAskedFor() {
      var source = new IsinSource("DE");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith("DE");
         IdentifierValidation.IsValidIsin(value).ShouldBeTrue();
      }
   }

   /// <summary>
   ///   An invented country prefix would make the number impossible, so only real ones are drawn.
   /// </summary>
   [Fact]
   public void EveryDrawnPrefixIsARealCountryOrTheInternationalCode() {
      var known = new[] {
         "AT", "AU", "BE", "CA", "CH", "CZ", "DE", "DK", "ES", "FI", "FR", "GB", "IE", "IT",
         "JP", "LU", "NL", "NO", "PL", "PT", "SE", "US", "XS"
      };

      var source = new IsinSource();
      var drawn = Enumerable.Range(0, 2000).Select(_ => source.Next(null)[..2]).Distinct().OrderBy(p => p).ToList();

      drawn.Where(p => !known.Contains(p)).ShouldBeEmpty();
      known.Where(p => !drawn.Contains(p)).ShouldBeEmpty("not every known prefix was drawn");
   }

   [Theory]
   [InlineData("D")]
   [InlineData("DEU")]
   [InlineData("de")]
   [InlineData("D1")]
   [InlineData("")]
   public void AnImpossiblePrefixIsRejected(string prefix) {
      var ex = Should.Throw<ArgumentException>(() => new IsinSource(prefix));
      ex.ParamName.ShouldBe("prefix");
   }

   [Fact]
   public void EveryCheckDigitFromZeroToNineOccurs() {
      var source = new IsinSource();

      Enumerable.Range(0, 2000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ShouldBe("0123456789".ToCharArray());
   }

   [Fact]
   public void NextReturnsStableIsinListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new IsinSource(), new string[] { "LU3KJWNS2YH7", "ES63RKZQQQI4", "ESU5WG3T65L0", "FRWTMG9VIY56", "IEL4ZVP1C4S7", "CAAP6RM9HDQ0", "SER3G6QN1WG5", "CZXVSM3VCLQ6", "DKWJ4X3RRJF9", "JPGCEHI4U3V4" });

   [Fact]
   public void NextReturnsStableIsinListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableIsinSource()!, new string?[] { "LU3KJWNS2YH7", "ES63RKZQQQI4", "ESU5WG3T65L0", "FRWTMG9VIY56", "IEL4ZVP1C4S7", "CAAP6RM9HDQ0", "SER3G6QN1WG5", "CZXVSM3VCLQ6", "DKWJ4X3RRJF9", "JPGCEHI4U3V4", "ES3SJ4SUGDG1", "ATO0FP1L8CX5", "ITWBQ679NTJ8", "GB14YJM7A2W7", "CHVB0CN5WVJ7", "CZDM0NN8LWS7", "PL3PNKGGHNC0", "NONANPMU0BV0", "NLHPCZ0XSDJ0", "NLDCB2STBYF6", "IE83L4YFJ5P9", "AUVXZT1IK5S2", "AUTO7USF3130", "CHVUE17F56I8", null, "CZEZYSREZH25", "FI9B0GJ7SQG9", null, "CZF0WASWYK21", "CA2STWIZ3U02" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableIsinSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidIsin(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new IsinSource().Next(null)).ShouldAllBe(v => v != null);
}
