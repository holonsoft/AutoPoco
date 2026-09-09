using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class CompanySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableCompanyListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CompanySource(), new string[] { "Charles Townsend Agency", "Chotchkies", "Carrys Candles", "LexCorp", "Monks Diner", "Big T Burgers and Fries", "Keedsler Motors", "Wernham Hogg", "Moes Tavern", "C.H. Lavatory and Sons" });

   [Fact]
   public void NextReturnsStableCompanyListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCompanySource()!, new string[] { "Charles Townsend Agency", "Chotchkies", "Carrys Candles", "LexCorp", "Monks Diner", "Big T Burgers and Fries", "Keedsler Motors", "Wernham Hogg", "Moes Tavern", "C.H. Lavatory and Sons", "Omni Consimer Products", "Charles Townsend Agency", "Barrytron", "The New Firm", "Transworld Consortium", "Gizmonic Institute", "Chasers", "Globo-Chem", "Thrift Bank", "Kumatsu Motors" });
}
