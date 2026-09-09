using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class PostalZipCodeGermanySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestability() {
      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeGermanySource(), new string[] { "40227", "14052", "30161", "99999", "14057", "40225", "29683", "13627", "20255", "30159" });

      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeNetherlandsSource(), new string[] { "1277VM", "0486FL", "9413XG", "7335MY", "8179PD", "0141KD", "4295YK", "1277VM", "6613WL", "6529IB" });

      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeUSASource(), new string[] { "66070", "05685", "63396", "58810", "29100", "68048", "09501", "66070", "12922", "88893" });
   }

   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestabilityAndListCanContainNull() {
      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeGermanySource()!, new string?[] { "40227", "14052", "30161", "99999", "14057", "40225", "29683", "13627", "20255", "30159", "99894", "40227", "19406", "14052", "20148", "19399", "30163", "13629", "01139", "39638", "20095", "01109", "20099", "99994", null, "40227", "29693", null, "99894", "99891" });

      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeNetherlandsSource()!, new string?[] { "1277VM", "0486FL", "9413XG", "7335MY", "8179PD", "0141KD", "4295YK", "1277VM", "6613WL", "6529IB", "9239RI", "4656VN", "9491BV", "3863CA", "1488DB", "0486FL", "8232MH", "9413XG", "6917LS", "7678AF", "7819HL", "3216HZ", "7678AF", "6056WK", null, "8070FC", "2505TE", null, "0997UB", "3769LI" });

      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeUSASource()!, new string?[] { "66070", "05685", "63396", "58810", "29100", "68048", "09501", "66070", "12922", "88893", "90617", "28937", "02380", "27809", "00377", "05685", "58249", "63396", "37880", "32279", "78946", "57428", "32279", "38011", null, "26395", "96788", null, "45429", "59426" });
   }
}
