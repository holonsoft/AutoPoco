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
         "Olivia", "Christopher", "Tyler", "Jason", "Zoe", "Eric", "Eric", "Sophia", "Abigail", "Ethan", "Nicholas", "Evelyn", "James", "Aria", "Zoe",
         "Claire", "Daniel", "Scott", null, null,
         "David", "Alexa", "Scarlett", "Joseph", "Kenneth", "David", "Matthew", "Scarlett", "Grace", "Michael"
         );

}