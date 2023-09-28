using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public partial class CreditCardSourceTests {
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
   public void NextReturnsARandom() {
      var source = new CreditCardSource();
      var value = source.Next(null).Replace(" ", "");
      value.Should().MatchRegex(_allowedCiphersDependingOnLengthRegex);
   }
}