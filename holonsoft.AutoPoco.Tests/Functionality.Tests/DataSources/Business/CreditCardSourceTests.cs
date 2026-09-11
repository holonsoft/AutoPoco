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

   [Fact]
   public void NextReturnsRandomCreditCardNumber() {
      var source = new CreditCardSource();
      var value = source.Next(null).Replace(" ", "");
      _allowedCiphersDependingOnLengthRegex.IsMatch(value).ShouldBeTrue();
   }

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CreditCardSource(), new string[] { "5343 1380 7226 1800", "4963 6432 2865 0035", "4655 0609 2255 2430", "6891 4832 9766 9177", "3430 671000 51867", "6657 6703 4138 0302", "5171 2438 3073 1141", "6030 2600 8079 1582", "5159 0467 9793 1279", "5238 6720 4807 5702" });

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCreditCardSource()!, new string[] { "5343 1380 7226 1800", "4963 6432 2865 0035", "4655 0609 2255 2430", "6891 4832 9766 9177", "3430 671000 51867", "6657 6703 4138 0302", "5171 2438 3073 1141", "6030 2600 8079 1582", "5159 0467 9793 1279", "5238 6720 4807 5702" });

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