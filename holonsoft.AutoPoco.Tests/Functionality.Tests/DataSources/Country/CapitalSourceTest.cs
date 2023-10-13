using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CapitalSourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CapitalSource(),
         "Ankara", "Stockholm", "Kuwait City", "Algiers", "Amman", "Nouakchott", "Nouakchott", "Athens", "Damascus", "Nairobi", "Riyadh", "Accra", "Hanoi",
         "Amsterdam", "Jerusalem", "Paris", "Abu Dhabi", "Rabat", "Manama", "Paris");

   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCapitalSource()!,
         new string?[] {
            "Ankara", "Stockholm", "Kuwait City", "Algiers", "Amman", "Nouakchott", "Nouakchott", "Athens", "Damascus", "Nairobi",
            "Riyadh", "Accra", "Hanoi", "Amsterdam", "Jerusalem", "Paris", "Abu Dhabi", "Rabat", null, null
         });
}