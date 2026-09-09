using Xunit;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class CapitalSourceTest() : TestBase {
   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new CapitalSource(), new string[] { "Pyongyang", "Kuwait City", "Manila", "Thimphu", "Shanghai", "Lisbon", "Seoul", "Muscat", "Tripoli", "Reykjavik", "Jerusalem", "Rabat", "Bangkok", "Pyongyang", "Beirut", "Ankara", "Ashgabat", "Manila", "Beirut", "Kuala Lumpur" });

   [Fact]
   public void NextReturnsStableCapitalListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableCapitalSource()!, new string[] { "Pyongyang", "Kuwait City", "Manila", "Thimphu", "Shanghai", "Lisbon", "Seoul", "Muscat", "Tripoli", "Reykjavik", "Jerusalem", "Rabat", "Bangkok", "Pyongyang", "Beirut", "Ankara", "Ashgabat", "Manila", "Beirut", "Kuala Lumpur" });
}