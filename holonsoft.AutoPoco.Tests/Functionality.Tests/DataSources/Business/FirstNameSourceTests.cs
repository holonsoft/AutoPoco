using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class FirstNameSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableFirstNameListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new FirstNameSource(), new string[] { "Daniel", "Zoey", "Sophia", "Victoria", "Aubrey", "George", "Ava", "Daniel", "Nora", "Scott" });

   [Fact]
   public void NextReturnsStableFirstNameListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableFirstNameSource()!, new string?[] { "Daniel", "Zoey", "Sophia", "Victoria", "Aubrey", "George", "Ava", "Daniel", "Nora", "Scott", "Ella", "Steven", "Anthony", "Abigail", "Scarlett", "Charles", "Zoey", "Amelia", "Sophia", "Alexander", "Joseph", "Charlotte", "Nathan", "Joseph", null, "Robert", "Sophie", null, "Owen", "Ethan" });

}