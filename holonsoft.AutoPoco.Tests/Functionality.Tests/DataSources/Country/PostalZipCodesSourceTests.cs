using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class PostalZipCodeGermanySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestability() {
      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeGermanySource(),
         "13627", "01156", "19399", "99988", "99894", "99994", "99994", "06279", "20099", "14059");

      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeNetherlandsSource(),
         "9413XG", "8728KV", "0459TW", "5101VF", "0190XX", "0141KD", "5724ZJ", "1742ZV", "1266DP", "6152SB");

      NextReturnsStableElementListInTermsOfTestability(
         new PostalZipCodeUSASource(),
         "63396", "38605", "77071", "67146", "63914", "68048", "62480", "47160", "91747", "42485");
   }

   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestabilityAndListCanContainNull() {
      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeGermanySource()!,
         "13627", "01156", "19399", "99988", "99894", "99994", "99994", "06279", "20099", "14059", "19395", "14057", "40219", "01108", "99894",
         "01069", "99958", "99988", null, null,
         "99955", "01067", "20253", "99947", "99991", "01156", "40215", "20251", "20149", "01139"
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeNetherlandsSource()!,
         "9413XG", "8728KV", "0459TW", "5101VF", "0190XX", "0141KD", "5724ZJ", "1742ZV", "1266DP", "6152SB", "0997UB", "4673YW", "2062NU", "3769LI",
         "0190XX", "6771ZY", "6576SS", "5101VF", null, null,
         "7865AL", "4603NG", "3080FO", "0344XA", "5724ZJ", "7541VY", "4069LA", "3080FO", "5984ML", "7541VY"
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullablePostalZipCodeUSASource()!,
         "63396", "38605", "77071", "67146", "63914", "68048", "62480", "47160", "91747", "42485", "45429", "10770", "95933", "59426", "63914", "47418",
         "71881", "67146", null, null,
         "26048", "60764", "42964", "61265", "62480", "93946", "66928", "42964", "46710", "93946"
         );
   }
}
