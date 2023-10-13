using FluentAssertions;
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
      value.Should().HaveLength(expectedLength).And.StartWith(startCipher);
      value.Replace(" ", "").Should().MatchRegex(_allowedCiphersDependingOnLengthRegex);
   }

   [Fact]
   public void NextReturnsRandomCreditCardNumber() {
      var source = new CreditCardSource();
      var value = source.Next(null).Replace(" ", "");
      value.Should().MatchRegex(_allowedCiphersDependingOnLengthRegex);
   }

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CreditCardSource(),
         new[] {
            "5139 8991 3232 7084", "5898 0489 1744 1883", "3695 475531 91051", "3224 797328 93124", "5358 3556 3251 4365", "3027 325203 91438",
            "5615 3395 0850 1892", "3644 657657 96148", "4440 3264 5197 1047", "5316 2174 7772 0575"
         });

   [Fact]
   public void NextReturnsStableCreditCardListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCreditCardSource()!,
         new string?[] {
            "5139 8991 3232 7084", null, "5898 0489 1744 1883", "3695 475531 91051", "3224 797328 93124", "5358 3556 3251 4365", "3027 325203 91438",
            "5615 3395 0850 1892", "3644 657657 96148", "4440 3264 5197 1047"
         });
}