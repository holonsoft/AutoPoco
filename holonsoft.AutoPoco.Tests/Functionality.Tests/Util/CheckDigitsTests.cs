using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

/// <summary>
///   Known answers for the two check digit algorithms, taken from published numbers. These pin the helpers
///   directly, without a data source in between, so a broken weight shows up here first.
/// </summary>
public class CheckDigitsTests {
   [Theory]
   [InlineData("9638507", 4)]          // EAN-8 96385074
   [InlineData("03600029145", 2)]      // UPC-A 036000291452
   [InlineData("400638133393", 1)]     // EAN-13 4006381333931, Faber-Castell
   [InlineData("590123412345", 7)]     // EAN-13 5901234123457, the GS1 example
   [InlineData("978316148410", 0)]     // ISBN-13 9783161484100, check digit zero
   [InlineData("1061414100041", 5)]    // GTIN-14 10614141000415
   public void Gs1Mod10ReturnsTheCheckDigitOfAPublishedNumber(string dataDigits, int expected)
      => CheckDigits.Gs1Mod10(dataDigits, dataDigits.Length).ShouldBe(expected);

   [Theory]
   [InlineData("030640615", 2)]        // ISBN-10 0306406152, The C Programming Language
   [InlineData("019852663", 6)]        // ISBN-10 0198526636
   [InlineData("080442957", 10)]       // ISBN-10 080442957X, check digit ten
   public void Mod11DescendingReturnsTheCheckDigitOfAPublishedNumber(string dataDigits, int expected)
      => CheckDigits.Mod11Descending(dataDigits, dataDigits.Length).ShouldBe(expected);

   [Theory]
   [InlineData("7992739871", 3)]          // 79927398713, the textbook Luhn example
   [InlineData("411111111111111", 1)]     // 4111111111111111, the Visa test card
   [InlineData("550000555555555", 9)]     // 5500005555555559, the MasterCard test card
   [InlineData("37828224631000", 5)]      // 378282246310005, the American Express test card
   public void LuhnReturnsTheCheckDigitOfAPublishedNumber(string dataDigits, int expected)
      => CheckDigits.Luhn(dataDigits, dataDigits.Length).ShouldBe(expected);

   /// <summary>
   ///   Computing a check digit doubles the digit directly left of it. Running the validation parity over a
   ///   number that has no check digit yet shifts every weight by one position and produces a number that
   ///   fails every real validator, which is the defect this helper was extracted to fix.
   /// </summary>
   [Fact]
   public void TheLuhnWeightsStartAtTheDigitLeftOfTheCheckDigit() {
      CheckDigits.Luhn("0", 1).ShouldBe(0);    // 0 doubled stays 0, nothing to lift
      CheckDigits.Luhn("1", 1).ShouldBe(8);    // 1 doubled is 2, so 8 lifts the sum to 10
      CheckDigits.Luhn("5", 1).ShouldBe(9);    // 5 doubled is 10, minus nine is 1, so 9 lifts it to 10
   }

   /// <summary>
   ///   Zero is a check digit, not the absence of one. Both algorithms have to fold the ten back to zero
   ///   instead of returning it.
   /// </summary>
   [Fact]
   public void ACheckDigitOfTenFoldsBackToZero() {
      CheckDigits.Gs1Mod10("000000000000", 12).ShouldBe(0);
      CheckDigits.Mod11Descending("000000000", 9).ShouldBe(0);
   }

   /// <summary>
   ///   The GS1 weights alternate from the right, so the same digits in a different length get a different
   ///   check digit. A weighting applied from the left would return the same value for both.
   /// </summary>
   [Fact]
   public void TheGs1WeightsStartAtTheRightEnd() {
      CheckDigits.Gs1Mod10("1000000", 7).ShouldBe(7);        // the 1 is weighted 3
      CheckDigits.Gs1Mod10("10000000", 8).ShouldBe(9);       // one digit longer, now the 1 is weighted 1
   }

   /// <summary>
   ///   Every possible check digit has to be produced by some input, otherwise part of the value range is
   ///   unreachable.
   /// </summary>
   [Fact]
   public void EveryCheckDigitIsReachable() {
      var gs1 = Enumerable.Range(0, 1000).Select(i => CheckDigits.Gs1Mod10(i.ToString("D12"), 12)).Distinct().OrderBy(d => d);
      gs1.ShouldBe(Enumerable.Range(0, 10));

      var isbn = Enumerable.Range(0, 1000).Select(i => CheckDigits.Mod11Descending(i.ToString("D9"), 9)).Distinct().OrderBy(d => d);
      isbn.ShouldBe(Enumerable.Range(0, 11));
   }
}
