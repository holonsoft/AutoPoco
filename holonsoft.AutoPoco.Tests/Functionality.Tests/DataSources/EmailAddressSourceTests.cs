using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class EmailAddressSourceTests : FixedStringArrayDataSourceTestBase<EmailAddressSource> {
   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability() {
      var source = new EmailAddressSource();
      var email = source.Next(null);
      email.Should().NotBeNull();

      source = new EmailAddressSource();
      var emailOne = source.Next(null);
      var emailTwo = source.Next(null);
      emailOne.Should().NotBe(emailTwo);

      PerformTest("eg0@example.com", "eg1@example.com", "eg2@example.com", "eg3@example.com", "eg4@example.com", "eg5@example.com", "eg6@example.com", "eg7@example.com", "eg8@example.com", "eg9@example.com");
   }
}
