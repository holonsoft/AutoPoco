using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class ExtendedEmailAddressSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new ExtendedEmailAddressSource(),
         "Olivia.Turner@holonsoft.invalid", "Christopher.Martin@hotmail.invalid", "Tyler.Murphy@golem.invalid", "Jason.Pierce@microsoft.example",
         "Zoe.Wells@yahoo.example", "Eric.Scott@heise.example", "Eric.Scott@heise.example", "Sophia.Baker@yahoo.invalid", "Abigail.Perez@msn.test",
         "Ethan.Evans@heise.invalid"
         );

   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestabilityAndListCanContainNull() => NextReturnsStableElementListInTermsOfTestability(
         new NullableExtendedEmailAddressSource()!,
         "Olivia.Turner@holonsoft.invalid", null, "Christopher.Martin@hotmail.invalid", "Tyler.Murphy@golem.invalid", "Jason.Pierce@microsoft.example",
         "Zoe.Wells@yahoo.example", "Eric.Scott@heise.example", "Eric.Scott@heise.example", "Sophia.Baker@yahoo.invalid", "Abigail.Perez@msn.test"
         );
}