using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class ExtendedEmailAddressSourceTests : FixedStringArrayDataSourceTestBase<ExtendedEmailAddressSource> {
   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability() {
      var source = new ExtendedEmailAddressSource();
      var emailOne = source.Next(null);
      var emailTwo = source.Next(null);
      emailOne.Should().NotBe(emailTwo);

      PerformTest("George.Turner@msn.nl", "Daniel.Martin@msn.nl", "Mohammed.Murphy@paspar.nl", "Evelin.Pierce@golem.de", "Luna.Wells@heise.de", "David.Scott@golem.de", "David.Scott@golem.de", "Samuel.Baker@msn.nl", "Ruby.Perez@hotmail.com", "Lewis.Evans@paspar.nl");
   }
}