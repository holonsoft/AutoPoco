using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class GtinSourceTests : TestBase {
   /// <summary>
   ///   Real barcode numbers, so the validator is measured against the world before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("96385074")]           // EAN-8, the textbook example
   [InlineData("036000291452")]       // UPC-A of a pack of chewing gum
   [InlineData("4006381333931")]      // EAN-13 of a Faber-Castell pen
   [InlineData("5901234123457")]      // EAN-13, the GS1 example number
   [InlineData("9783161484100")]      // ISBN-13, which is an EAN-13 as well
   [InlineData("10614141000415")]     // GTIN-14 of a carton
   public void TheValidatorAcceptsRealGtins(string value)
      => IdentifierValidation.IsValidGtin(value).ShouldBeTrue();

   [Theory]
   [InlineData("4006381333932")]      // last digit off by one
   [InlineData("4006381333941")]      // a data digit off by one
   [InlineData("0306406152")]         // an ISBN-10, not a GTIN length
   [InlineData("400638")]             // no GTIN is six digits long
   [InlineData("400638133393")]       // a valid GTIN-12 length, so this one fails on the checksum, not on the length
   [InlineData("400638133393X")]      // right length, not a digit
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsBrokenGtins(string? value)
      => IdentifierValidation.IsValidGtin(value).ShouldBeFalse();

   [Theory]
   [InlineData(GtinFormat.Gtin8, 8)]
   [InlineData(GtinFormat.Gtin12, 12)]
   [InlineData(GtinFormat.Gtin13, 13)]
   [InlineData(GtinFormat.Gtin14, 14)]
   public void EveryFormatHasItsLengthAndAValidCheckDigit(GtinFormat format, int expectedLength) {
      var source = new GtinSource(format);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(expectedLength);
         value.ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void TheDefaultFormatIsGtin13() {
      var value = new GtinSource().Next(null);
      value.Length.ShouldBe(13);
      IdentifierValidation.IsValidGtin(value).ShouldBeTrue();
   }

   [Fact]
   public void APrefixStartsEveryNumberAndTheRestStillValidates() {
      var source = new GtinSource(GtinFormat.Gtin13, "40063");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.ShouldStartWith("40063");
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue();
      }
   }

   [Fact]
   public void Ean13SourceTakesAPrefixAsWell() {
      var source = new Ean13Source("40063");

      for (var i = 0; i < 100; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(13);
         value.ShouldStartWith("40063");
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue();
      }
   }

   /// <summary>
   ///   Every constructor overload has to end up at the same behaviour, a valid number of the asked for
   ///   format. An overload that forwards a wrong default would be invisible otherwise.
   /// </summary>
   [Fact]
   public void EveryNullableOverloadProducesValidNumbers() {
      AllNonNullValuesAreValidGtins(new NullableGtinSource(), 13);
      AllNonNullValuesAreValidGtins(new NullableGtinSource(GtinFormat.Gtin8), 8);
      AllNonNullValuesAreValidGtins(new NullableGtinSource(GtinFormat.Gtin14, "1234"), 14, "1234");
      AllNonNullValuesAreValidGtins(new NullableGtinSource(50), 13);
      AllNonNullValuesAreValidGtins(new NullableEan13Source(), 13);
      AllNonNullValuesAreValidGtins(new NullableEan13Source("40063"), 13, "40063");
      AllNonNullValuesAreValidGtins(new NullableEan13Source(50), 13);
   }

   private static void AllNonNullValuesAreValidGtins(GtinSourceBase source, int expectedLength, string? expectedPrefix = null) {
      var values = Enumerable.Range(0, 200).Select(_ => source.Next(null)).Where(v => v is not null).ToList();

      values.ShouldNotBeEmpty();

      foreach (var value in values) {
         value.Length.ShouldBe(expectedLength);
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");

         if (expectedPrefix is not null)
            value.ShouldStartWith(expectedPrefix);
      }
   }

   [Fact]
   public void APrefixMayFillEverythingButTheCheckDigit() {
      // twelve of thirteen digits given, only the check digit is left to calculate
      var source = new GtinSource(GtinFormat.Gtin13, "400638133393");

      for (var i = 0; i < 10; i++)
         source.Next(null).ShouldBe("4006381333931");
   }

   [Fact]
   public void APrefixLongerThanTheFormatIsRejected() {
      var ex = Should.Throw<ArgumentException>(() => new GtinSource(GtinFormat.Gtin8, "12345678"));
      ex.ParamName.ShouldBe("prefix");
   }

   [Theory]
   [InlineData("40063a")]
   [InlineData("400 63")]
   [InlineData("-4006")]
   [InlineData("4006.3")]
   public void APrefixThatIsNotAllDigitsIsRejected(string prefix) {
      var ex = Should.Throw<ArgumentException>(() => new GtinSource(GtinFormat.Gtin13, prefix));
      ex.ParamName.ShouldBe("prefix");
   }

   [Fact]
   public void AnEmptyPrefixIsTreatedAsNoPrefix()
      => IdentifierValidation.IsValidGtin(new GtinSource(GtinFormat.Gtin13, "").Next(null)).ShouldBeTrue();

   [Fact]
   public void AnUnknownFormatIsRejected() {
      var ex = Should.Throw<ArgumentOutOfRangeException>(() => new GtinSource((GtinFormat) 11));
      ex.ParamName.ShouldBe("format");
   }

   /// <summary>
   ///   A check digit is only worth something when it catches a typo. Changing any single digit of a
   ///   generated number has to break it, which a weight applied in the wrong direction would not do.
   /// </summary>
   [Fact]
   public void NoSingleDigitChangeOfAGeneratedNumberStillValidates() {
      var source = new GtinSource(GtinFormat.Gtin13);

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, false))
            IdentifierValidation.IsValidGtin(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   /// <summary>
   ///   Every check digit from 0 to 9 has to be reachable. CreditCardSource once shipped with a source
   ///   that could never produce one of its card types, this is the guard against the same kind of bug.
   /// </summary>
   [Fact]
   public void EveryCheckDigitFromZeroToNineOccurs() {
      var source = new GtinSource(GtinFormat.Gtin13);

      var seen = Enumerable.Range(0, 2000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ToList();

      seen.ShouldBe("0123456789".ToCharArray());
   }

   [Fact]
   public void NextReturnsStableGtinListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new GtinSource(), new string[] { "3431380722614", "8096364322867", "5003655060929", "2552438914834", "2976691743060", "7100051863654", "7670341380302", "1712438307314", "1403026008071", "9158159046798" });

   [Fact]
   public void NextReturnsStableEan13ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new Ean13Source(), new string[] { "3431380722614", "8096364322867", "5003655060929", "2552438914834", "2976691743060", "7100051863654", "7670341380302", "1712438307314", "1403026008071", "9158159046798" });

   [Fact]
   public void NextReturnsStableGtin8ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new GtinSource(GtinFormat.Gtin8), new string[] { "34313800", "72261804", "96364321", "28650034", "65506097", "22552433", "89148327", "97669173", "43067107", "00518635" });

   [Fact]
   public void NextReturnsStableGtin12ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new GtinSource(GtinFormat.Gtin12), new string[] { "343138072267", "180963643220", "865003655066", "092255243899", "148329766914", "743067100053", "186365767038", "413803017128", "438307311404", "302600807912" });

   [Fact]
   public void NextReturnsStableGtin14ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new GtinSource(GtinFormat.Gtin14), new string[] { "34313807226182", "09636432286500", "03655060922556", "24389148329762", "69174306710000", "51863657670347", "13803017124387", "30731140302600", "08079158159047", "67979312742388" });

   [Fact]
   public void NextReturnsStableGtinListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableGtinSource()!, new string?[] { "3431380722614", "8096364322867", "5003655060929", "2552438914834", "2976691743060", "7100051863654", "7670341380302", "1712438307314", "1403026008071", "9158159046798", "7931274238673", "2048075703554", "6096397678506", "0293897940072", "8176180727975", "9686097701993", "3013062832868", "7899735854216", "3859191312457", "1787313441155", "7778656253219", "3312999987079", "0367705200020", "4273280230306", null, "3924925135616", "3600922413093", null, "0076477167188", "9292880778426" });

   [Fact]
   public void NextReturnsStableEan13ListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableEan13Source()!, new string?[] { "3431380722614", "8096364322867", "5003655060929", "2552438914834", "2976691743060", "7100051863654", "7670341380302", "1712438307314", "1403026008071", "9158159046798", "7931274238673", "2048075703554", "6096397678506", "0293897940072", "8176180727975", "9686097701993", "3013062832868", "7899735854216", "3859191312457", "1787313441155", "7778656253219", "3312999987079", "0367705200020", "4273280230306", null, "3924925135616", "3600922413093", null, "0076477167188", "9292880778426" });

   /// <summary>
   ///   The default threshold produces a null so rarely that a short sequence never shows one. At an
   ///   aggressive threshold both halves of the behaviour have to be visible: nulls appear, and everything
   ///   that is not null is still a valid GTIN.
   /// </summary>
   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableGtinSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANullableSourceKeepsTheFormatAndThePrefix() {
      var source = new NullableGtinSource(GtinFormat.Gtin8, "123", 50);

      foreach (var value in Enumerable.Range(0, 300).Select(_ => source.Next(null)).Where(v => v is not null)) {
         value.Length.ShouldBe(8);
         value.ShouldStartWith("123");
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue();
      }
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new GtinSource().Next(null)).ShouldAllBe(v => v != null);
}
