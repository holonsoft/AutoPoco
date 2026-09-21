using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using static holonsoft.AutoPoco.DataSources.Business.CreditCardSourceBase;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class TestBinCreditCardSourceTests : TestBase {
   /// <summary>
   ///   The same independent Luhn check as in <see cref="CreditCardSourceTests" />, so this file stands on
   ///   its own.
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
   ///   The whole point of the source: the number has to be recognisable as test data. These are the numbers
   ///   the processors publish, and the source has to build on exactly their first six digits.
   /// </summary>
   [Theory]
   [InlineData("4111111111111111")]
   [InlineData("4242424242424242")]
   [InlineData("4012888888881881")]
   [InlineData("4000056655665556")]
   [InlineData("5555555555554444")]
   [InlineData("5105105105105100")]
   [InlineData("5500005555555559")]
   [InlineData("2223003122003222")]
   [InlineData("378282246310005")]
   [InlineData("371449635398431")]
   [InlineData("378734493671000")]
   [InlineData("6011111111111117")]
   [InlineData("6011000990139424")]
   [InlineData("6011981111111113")]
   public void EveryBinComesFromAPublishedTestCard(string publishedCard) {
      IsValidLuhn(publishedCard).ShouldBeTrue($"'{publishedCard}' is not a valid test card number");
      TestBinCreditCardSourceBase.PublishedTestBins.ShouldContain(publishedCard[..6]);
   }

   [Theory]
   [InlineData(CreditCardType.AmericanExpress, 15)]
   [InlineData(CreditCardType.Discover, 16)]
   [InlineData(CreditCardType.MasterCard, 16)]
   [InlineData(CreditCardType.Visa, 16)]
   public void EveryNumberHasItsLengthAndAValidCheckDigit(CreditCardType cardType, int expectedLength) {
      var source = new TestBinCreditCardSource(cardType);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null).Replace(" ", "");
         value.Length.ShouldBe(expectedLength);
         IsValidLuhn(value).ShouldBeTrue($"'{value}' does not pass the Luhn check");
      }
   }

   [Fact]
   public void EveryNumberStartsWithAPublishedTestBin() {
      var source = new TestBinCreditCardSource();

      for (var i = 0; i < 500; i++) {
         var value = source.Next(null).Replace(" ", "");
         TestBinCreditCardSourceBase.PublishedTestBins.ShouldContain(value[..6], $"'{value}' is not on a test BIN");
      }
   }

   /// <summary>
   ///   A card type still means what it meant. MasterCard is the one that needs two digits: besides the old
   ///   five series it also hands out the two series, from 2221 to 2720, so a MasterCard can start with a two.
   /// </summary>
   [Theory]
   [InlineData(CreditCardType.AmericanExpress, "3")]
   [InlineData(CreditCardType.Discover, "6")]
   [InlineData(CreditCardType.MasterCard, "52")]
   [InlineData(CreditCardType.Visa, "4")]
   public void TheSchemeDigitStillMatchesTheCardType(CreditCardType cardType, string allowedFirstDigits) {
      var source = new TestBinCreditCardSource(cardType);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         allowedFirstDigits.ShouldContain(value[0], $"'{value}' does not start like a {cardType}");
      }
   }

   /// <summary>
   ///   MasterCard reaches into the 2 series as well, which is why the scheme digit alone is not the whole
   ///   story and this case is called out separately.
   /// </summary>
   [Fact]
   public void TheMasterCardTwoSeriesIsReachable() {
      var source = new TestBinCreditCardSource(CreditCardType.MasterCard);

      Enumerable.Range(0, 500)
         .Select(_ => source.Next(null).Replace(" ", "")[..6])
         .Distinct()
         .ShouldContain("222300");
   }

   [Fact]
   public void EveryPublishedTestBinIsReachable() {
      var source = new TestBinCreditCardSource();

      var drawn = Enumerable.Range(0, 5000)
         .Select(_ => source.Next(null).Replace(" ", "")[..6])
         .Distinct()
         .ToList();

      TestBinCreditCardSourceBase.PublishedTestBins
         .Where(b => !drawn.Contains(b))
         .ShouldBeEmpty("not every published test BIN was drawn");
   }

   /// <summary>
   ///   The plain source draws everything behind the scheme digit, this one does not. Two sources on the same
   ///   seed therefore have to disagree, otherwise the override is not in place.
   /// </summary>
   [Fact]
   public void TheNumbersDifferFromThePlainCreditCardSource() {
      var plain = new CreditCardSource(CreditCardType.Visa);
      var onTestBins = new TestBinCreditCardSource(CreditCardType.Visa);

      var fromPlain = Enumerable.Range(0, 20).Select(_ => plain.Next(null)).ToList();
      var fromTestBins = Enumerable.Range(0, 20).Select(_ => onTestBins.Next(null)).ToList();

      fromTestBins.ShouldNotBe(fromPlain);
   }

   [Fact]
   public void NextReturnsStableCardListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new TestBinCreditCardSource(), new string[] { "2223 0034 3138 0725", "3782 826180 96364", "2223 0022 8650 0361", "4242 4206 0922 5529", "6011 1138 9148 3294", "2223 0066 9174 3060", "3787 347100 05180", "3787 345767 03415", "6011 1103 0171 2437", "6011 1130 7311 4030" });

   [Fact]
   public void NextReturnsStableCardListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableTestBinCreditCardSource()!, new string?[] { "2223 0034 3138 0725", "3782 826180 96364", "2223 0022 8650 0361", "4242 4206 0922 5529", "6011 1138 9148 3294", "2223 0066 9174 3060", "3787 347100 05180", "3787 345767 03415", "6011 1103 0171 2437", "6011 1130 7311 4030", "2223 0026 0080 7914", "6011 0081 5904 6793", "6011 0031 2742 3869", "6011 9820 4807 5702", "6011 0056 0963 9763", "6011 1150 0293 8975", "4111 1100 7817 6182", "2223 0027 9796 8602", "4000 0577 0199 3018", "5105 1030 6283 2865", "6011 1178 9973 5852", "5500 0013 8591 9131", "4242 4224 5178 7317", "6011 1141 1577 7867", null, "6011 0062 5321 3319", "6011 9899 9987 0700", null, "6011 9877 0520 0022", "5500 0073 2802 3030" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableTestBinCreditCardSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IsValidLuhn(value.Replace(" ", "")).ShouldBeTrue($"'{value}' does not pass the Luhn check");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new TestBinCreditCardSource().Next(null)).ShouldAllBe(v => v != null);
}
