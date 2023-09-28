using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class PostalZipCodeGermanySourceTest : FixedStringArrayDataSourceTest<PostalZipCodeGermanySource> {
   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestability()
      => PerformTest("13627", "01156", "19399", "99988", "99894", "99994", "99994", "06279", "20099", "14059");
}

public class PostalZipCodeDutchSourceTest : FixedStringArrayDataSourceTest<PostalZipCodeDutchSource> {
   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestability()
      => PerformTest("9413XG", "8728KV", "0459TW", "5101VF", "0190XX", "0141KD", "5724ZJ", "1742ZV", "1266DP", "6152SB");
}

public class PostalZipCodeUSASourceTest : FixedStringArrayDataSourceTest<PostalZipCodeUSASource> {
   [Fact]
   public void NextReturnsStableZipCodeListsInTermsOfTestability()
      => PerformTest("63396", "38605", "77071", "67146", "63914", "68048", "62480", "47160", "91747", "42485");
}