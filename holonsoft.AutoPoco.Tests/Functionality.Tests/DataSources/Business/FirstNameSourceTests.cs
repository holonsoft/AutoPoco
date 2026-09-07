using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class FirstNameSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableFirstNameListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new FirstNameSource(),
         "Olivia", "Christopher", "Tyler", "Jason", "Zoe", "Eric", "Eric", "Sophia", "Abigail", "Ethan");

   [Fact]
   public void NextReturnsStableFirstNameListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableFirstNameSource()!,
         "Olivia", null, "Christopher", "Tyler", "Jason", "Zoe", "Eric", "Eric", "Sophia", "Abigail",
         "Ethan", "Nicholas", "Evelyn", null, "James", null, "Aria", "Zoe", "Claire", null,
         "Daniel", "Scott", "David", null, "Alexa", "Scarlett", "Joseph", null, "Kenneth", "David"
         );

}