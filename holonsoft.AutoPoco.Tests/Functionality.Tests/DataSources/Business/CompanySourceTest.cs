using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class CompanySourceTest : TestBase {
   [Fact]
   public void NextReturnsStableCompanyListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CompanySource(),
         new[] {
            "Sto Plains Holdings", "Ankh-Sto Associates", "Gringotts", "Minuteman Cafe", "Niagular", "Moes Tavern", "Moes Tavern",
            "Praxis Corporation", "Kumatsu Motors", "Mammoth Pictures"
         });

   [Fact]
   public void NextReturnsStableCompanyListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCompanySource()!,
         new string?[] {
            "Sto Plains Holdings", "Ankh-Sto Associates", "Gringotts", "Minuteman Cafe", "Niagular", "Moes Tavern", "Moes Tavern", "Praxis Corporation",
            "Kumatsu Motors", "Mammoth Pictures", "Mammoth Pictures", "Water and Power", "Rouster and Sideways", "ABC Telecom", "Niagular", "Widget Corp",
            "Chotchkies", "Minuteman Cafe", null, null
         });
}
