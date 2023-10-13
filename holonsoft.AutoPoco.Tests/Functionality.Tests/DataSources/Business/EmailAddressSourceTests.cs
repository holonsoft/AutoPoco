using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class EmailAddressSourceTests : TestBase {
   [Fact]
   public void NextReturnsDifferentEmailsWhenChangingSeed() {
      var source = new EmailAddressSource();
      var emailOne = source.Next(null);
      emailOne.Should().NotBeNull();

      source.SetSeedToRandomValue(10212);
      var emailTwo = source.Next(null);
      emailTwo.Should().NotBeNull();
      emailOne.Should().NotBe(emailTwo);
   }

   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new EmailAddressSource(),
         "eg0@example.test", "eg1@example.test", "eg2@example.test", "eg3@example.test", "eg4@example.test", "eg5@example.test", "eg6@example.test",
         "eg7@example.test", "eg8@example.test", "eg9@example.test"
         );

   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableEmailAddressSource()!,
         "eg0@example.test", null, "eg1@example.test", "eg2@example.test", "eg3@example.test", "eg4@example.test", "eg5@example.test", "eg6@example.test",
         "eg7@example.test", "eg8@example.test", "eg9@example.test", "eg10@example.test", "eg11@example.test", null,
         "eg12@example.test", null, "eg13@example.test", "eg14@example.test", "eg15@example.test"
         );
}
