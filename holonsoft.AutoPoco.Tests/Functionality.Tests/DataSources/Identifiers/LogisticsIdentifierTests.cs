using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

/// <summary>
///   The SSCC of a logistic unit. It carries the same GS1 mod 10 check digit as a GTIN, and that arithmetic is
///   already anchored against six published barcodes in <see cref="GtinSourceTests" />, so these tests pin the
///   length, the prefix and the check digit rather than repeating the anchor.
/// </summary>
public class SsccSourceTests : TestBase {
   [Fact]
   public void EverySsccHasEighteenDigitsAndAValidCheckDigit() {
      var source = new SsccSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(18);
         value.ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidSscc(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void APrefixStartsEveryNumber() {
      var source = new SsccSource("3401234");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith("3401234");
         IdentifierValidation.IsValidSscc(value).ShouldBeTrue();
      }
   }

   [Fact]
   public void APrefixMayFillEverythingButTheCheckDigit() {
      var source = new SsccSource("34012345000000001");

      for (var i = 0; i < 5; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(18);
         IdentifierValidation.IsValidSscc(value).ShouldBeTrue();
         value.ShouldBe(source.Next(null));
      }
   }

   [Theory]
   [InlineData("3401234a")]
   [InlineData("340 1234")]
   [InlineData("123456789012345678")]
   public void AnImpossiblePrefixIsRejected(string prefix) {
      var ex = Should.Throw<ArgumentException>(() => new SsccSource(prefix));
      ex.ParamName.ShouldBe("prefix");
   }

   [Fact]
   public void NoSingleDigitChangeOfAGeneratedNumberStillValidates() {
      var source = new SsccSource();

      for (var i = 0; i < 10; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, false))
            IdentifierValidation.IsValidSscc(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   [Fact]
   public void EveryCheckDigitFromZeroToNineOccurs() {
      var source = new SsccSource();

      Enumerable.Range(0, 2000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ShouldBe("0123456789".ToCharArray());
   }

   [Fact]
   public void NextReturnsStableSsccListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new SsccSource(), new string[] { "343138072261809630", "643228650036550609", "922552438914832975", "669174306710005186", "636576703413803013", "712438307311403027", "600807915815904672", "979312742386720486", "075703556096397671", "850029389794007812" });

   [Fact]
   public void NextReturnsStableSsccListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableSsccSource()!, new string?[] { "343138072261809630", "643228650036550609", "922552438914832975", "669174306710005186", "636576703413803013", "712438307311403027", "600807915815904672", "979312742386720486", "075703556096397671", "850029389794007812", "761807279796860975", "701993013062832868", "789973585421385918", "913124517873134418", "157778656253213312", "299998707036770528", "000242732802303037", "924925135613600923", "241309007647716719", "892928807784235714", "943461458284137405", "277562078514892498", "766760445823633903", "252526120707811580", null, "808193637895877496", "813341129582445518", null, "673746367001924430", "521286182530958179" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableSsccSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidSscc(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new SsccSource().Next(null)).ShouldAllBe(v => v != null);
}
