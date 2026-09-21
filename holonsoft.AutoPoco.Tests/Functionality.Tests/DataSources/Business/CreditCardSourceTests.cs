using Shouldly;
using System.Text.RegularExpressions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public partial class CreditCardSourceTests : TestBase {
   // the classic validator patterns, one per scheme, the way a checkout form detects the card type:
   // Visa is everything on a 4, American Express 34 or 37, Mastercard 51 to 55 plus the 2 series
   // 2221 to 2720, Discover 6011, 644 to 649 and 65
   [GeneratedRegex(@"^4\d{15}$", RegexOptions.Compiled)]
   private static partial Regex VisaRegex();

   [GeneratedRegex(@"^3[47]\d{13}$", RegexOptions.Compiled)]
   private static partial Regex AmericanExpressRegex();

   [GeneratedRegex(@"^(5[1-5]\d{14}|2(22[1-9]|2[3-9]\d|[3-6]\d{2}|7[01]\d|720)\d{12})$", RegexOptions.Compiled)]
   private static partial Regex MasterCardRegex();

   [GeneratedRegex(@"^(6011\d{12}|64[4-9]\d{13}|65\d{14})$", RegexOptions.Compiled)]
   private static partial Regex DiscoverRegex();

   /// <summary>
   ///   The card type an unformatted number is detected as, null when no scheme claims its range.
   /// </summary>
   private static CreditCardSource.CreditCardType? SchemeOf(string number) {
      if (VisaRegex().IsMatch(number))
         return CreditCardSource.CreditCardType.Visa;
      if (AmericanExpressRegex().IsMatch(number))
         return CreditCardSource.CreditCardType.AmericanExpress;
      if (MasterCardRegex().IsMatch(number))
         return CreditCardSource.CreditCardType.MasterCard;
      if (DiscoverRegex().IsMatch(number))
         return CreditCardSource.CreditCardType.Discover;

      return null;
   }

   /// <summary>
   ///   The detection above is measured against published card numbers before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("4111111111111111", CreditCardSource.CreditCardType.Visa)]
   [InlineData("5555555555554444", CreditCardSource.CreditCardType.MasterCard)]
   [InlineData("2223003122003222", CreditCardSource.CreditCardType.MasterCard)]
   [InlineData("378282246310005", CreditCardSource.CreditCardType.AmericanExpress)]
   [InlineData("6011111111111117", CreditCardSource.CreditCardType.Discover)]
   [InlineData("6011981111111113", CreditCardSource.CreditCardType.Discover)]
   [InlineData("36227206271667", null)]     // Diners Club, no card type of this source
   [InlineData("3530111333300000", null)]   // JCB, no card type of this source
   public void TheSchemeDetectionRecognisesPublishedCards(string number, CreditCardSource.CreditCardType? expected)
      => SchemeOf(number).ShouldBe(expected);

   /// <summary>
   ///   Up to 6.0 only the first digit of the scheme was fixed, so an "American Express" could start with 36
   ///   and be detected as a Diners Club, and a "MasterCard" could start with 56, which no scheme issues.
   ///   Now every number stays inside a range its scheme really issues in.
   /// </summary>
   [Theory]
   [InlineData(CreditCardSource.CreditCardType.AmericanExpress, 17)]
   [InlineData(CreditCardSource.CreditCardType.Discover, 19)]
   [InlineData(CreditCardSource.CreditCardType.MasterCard, 19)]
   [InlineData(CreditCardSource.CreditCardType.Visa, 19)]
   public void EveryNumberStaysInsideTheRangeOfItsScheme(CreditCardSource.CreditCardType cardType, int expectedLength) {
      var source = new CreditCardSource(cardType);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(expectedLength);
         SchemeOf(value.Replace(" ", "")).ShouldBe(cardType, $"'{value}' is not detected as a {cardType}");
      }
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
      SchemeOf(value).ShouldNotBeNull();
   }

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CreditCardSource(), new string[] { "2288 4313 8072 2617", "5263 6432 2865 0030", "4655 0609 2255 2439", "6011 9148 3297 6696", "4743 0671 0005 1864", "6557 6703 4138 0305", "5271 2438 3073 1144", "6011 0302 6008 0792", "4581 5904 6797 9315", "3774 238672 04801" });

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCreditCardSource()!, new string[] { "2288 4313 8072 2617", "5263 6432 2865 0030", "4655 0609 2255 2439", "6011 9148 3297 6696", "4743 0671 0005 1864", "6557 6703 4138 0305", "5271 2438 3073 1144", "6011 0302 6008 0792", "4581 5904 6797 9315", "3774 238672 04801" });

   [Fact]
   public void TheRandomCardTypeProducesEveryCardType() {
      // Random.Next(1, 4) stopped at AmericanExpress, so Discover was never generated
      var source = new CreditCardSource();
      var schemes = Enumerable.Range(0, 500)
         .Select(_ => SchemeOf(source.Next(null).Replace(" ", "")))
         .Distinct()
         .OrderBy(s => s)
         .ToList();

      schemes.ShouldBe([
         CreditCardSource.CreditCardType.MasterCard,
         CreditCardSource.CreditCardType.Visa,
         CreditCardSource.CreditCardType.AmericanExpress,
         CreditCardSource.CreditCardType.Discover
      ]);
   }
}