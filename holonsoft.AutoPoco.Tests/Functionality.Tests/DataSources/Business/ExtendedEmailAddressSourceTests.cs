using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class ExtendedEmailAddressSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new ExtendedEmailAddressSource(), new string[] { "Daniel.Knight@heise.example", "Zoey.Harrison@yahoo.test", "Sophia.Baker@hotmail.invalid", "Victoria.Diaz@google.example", "Aubrey.Harper@golem.test", "George.Dean@hotmail.test", "Ava.Thomas@microsoft.test", "Daniel.Knight@golem.test", "Nora.Long@aol.example", "Scott.Gordon@google.invalid" });

   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestabilityAndListCanContainNull() => NextReturnsStableElementListInTermsOfTestability(
         new NullableExtendedEmailAddressSource()!, new string[] { "Daniel.Knight@heise.example", "Zoey.Harrison@yahoo.test", "Sophia.Baker@hotmail.invalid", "Victoria.Diaz@google.example", "Aubrey.Harper@golem.test", "George.Dean@hotmail.test", "Ava.Thomas@microsoft.test", "Daniel.Knight@golem.test", "Nora.Long@aol.example", "Scott.Gordon@google.invalid" });
}