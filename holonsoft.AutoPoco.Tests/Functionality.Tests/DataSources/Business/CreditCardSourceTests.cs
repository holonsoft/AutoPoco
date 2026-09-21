using Shouldly;
using System.Text.RegularExpressions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public partial class CreditCardSourceTests : TestBase {
   private const string _regexString = @"(^(4|5|6)\d{15})|(^3\d{14})$";

   [GeneratedRegex(_regexString, RegexOptions.Compiled)]
   private static partial Regex AllowedCiphersDependingOnLengthRegex();
   private static readonly Regex _allowedCiphersDependingOnLengthRegex = AllowedCiphersDependingOnLengthRegex();

   [Theory]
   [InlineData(CreditCardSource.CreditCardType.AmericanExpress, "3", 17)]
   [InlineData(CreditCardSource.CreditCardType.Discover, "6", 19)]
   [InlineData(CreditCardSource.CreditCardType.MasterCard, "5", 19)]
   [InlineData(CreditCardSource.CreditCardType.Visa, "4", 19)]
   public void TestSeveralFakeCreditCardNumbers(CreditCardSource.CreditCardType preferredCreditCardType, string startCipher, int expectedLength) {
      var source = new CreditCardSource(preferredCreditCardType);
      var value = source.Next(null);
      value.Length.ShouldBe(expectedLength);
      value.ShouldStartWith(startCipher);
      _allowedCiphersDependingOnLengthRegex.IsMatch(value.Replace(" ", "")).ShouldBeTrue();
   }

   /// <summary>
   ///   Validates a complete card number the way a payment form does: the check digit itself is weighted one,
   ///   the digit left of it is doubled, and so on alternating to the left. Written the other way round than
   ///   the generator, which calculates the check digit from the data digits.
   /// </summary>
   private static bool IsValidLuhn(string number) {
      var sum = 0;
      var doubled = false;

      for (var i = number.Length - 1; i >= 0; i--) {
         var digit = number[i] - '0';

         if (doubled) {
            digit *= 2;
            if (digit > 9)
               digit -= 9;
         }

         sum += digit;
         doubled = !doubled;
      }

      return sum % 10 == 0;
   }

   /// <summary>
   ///   Published test card numbers, so the check above is measured against the world before it judges the
   ///   source.
   /// </summary>
   [Theory]
   [InlineData("4111111111111111")]   // Visa
   [InlineData("5500005555555559")]   // MasterCard
   [InlineData("378282246310005")]    // American Express
   [InlineData("6011111111111117")]   // Discover
   public void TheLuhnCheckAcceptsPublishedTestCards(string number)
      => IsValidLuhn(number).ShouldBeTrue();

   [Theory]
   [InlineData("4111111111111112")]
   [InlineData("5500005555555558")]
   public void TheLuhnCheckRejectsABrokenNumber(string number)
      => IsValidLuhn(number).ShouldBeFalse();

   /// <summary>
   ///   Up to 6.0 the generator ran the validation weights over a number that had no check digit yet, so every
   ///   digit was weighted one position off and not a single generated card passed a real Luhn check.
   /// </summary>
   [Theory]
   [InlineData(CreditCardSource.CreditCardType.AmericanExpress)]
   [InlineData(CreditCardSource.CreditCardType.Discover)]
   [InlineData(CreditCardSource.CreditCardType.MasterCard)]
   [InlineData(CreditCardSource.CreditCardType.Visa)]
   public void EveryGeneratedCardNumberPassesTheLuhnCheck(CreditCardSource.CreditCardType cardType) {
      var source = new CreditCardSource(cardType);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null).Replace(" ", "");
         IsValidLuhn(value).ShouldBeTrue($"'{value}' does not pass the Luhn check");
      }
   }

   [Fact]
   public void NextReturnsRandomCreditCardNumber() {
      var source = new CreditCardSource();
      var value = source.Next(null).Replace(" ", "");
      _allowedCiphersDependingOnLengthRegex.IsMatch(value).ShouldBeTrue();
   }

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CreditCardSource(), new string[] { "5343 1380 7226 1806", "4963 6432 2865 0036", "4655 0609 2255 2439", "6891 4832 9766 9175", "3430 671000 51866", "6657 6703 4138 0304", "5171 2438 3073 1145", "6030 2600 8079 1586", "5159 0467 9793 1277", "5238 6720 4807 5708" });

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCreditCardSource()!, new string[] { "5343 1380 7226 1806", "4963 6432 2865 0036", "4655 0609 2255 2439", "6891 4832 9766 9175", "3430 671000 51866", "6657 6703 4138 0304", "5171 2438 3073 1145", "6030 2600 8079 1586", "5159 0467 9793 1277", "5238 6720 4807 5708" });

   [Fact]
   public void TheRandomCardTypeProducesEveryCardType() {
      // Random.Next(1, 4) stopped at AmericanExpress, so Discover was never generated
      var source = new CreditCardSource();
      var firstDigits = Enumerable.Range(0, 500)
         .Select(_ => source.Next(null).Replace(" ", "")[0])
         .Distinct()
         .OrderBy(c => c)
         .ToList();

      firstDigits.ShouldBe(new[] { '3', '4', '5', '6' });
   }
}