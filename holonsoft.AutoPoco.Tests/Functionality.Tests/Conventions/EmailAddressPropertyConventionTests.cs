using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class EmailAddressPropertyConventionTests {
   private readonly EmailAddressPropertyConvention _convention = new();

   private TypeMemberConventionRequirements Requirements() {
      var requirements = new TypeMemberConventionRequirements();
      _convention.SpecifyRequirements(requirements);
      return requirements;
   }

   [Theory]
   [InlineData("EmailAddress", true)]
   [InlineData("emailaddress", true)]
   [InlineData("EMAILADDRESS", true)]
   [InlineData("Email", false)]
   [InlineData("EmailAddresses", false)]
   [InlineData("MailAddress", false)]
   [InlineData("", false)]
   public void RequirementsMatchTheNameCaseInsensitively(string name, bool expected)
      => Requirements().IsValidName(name).ShouldBe(expected);

   [Theory]
   [InlineData(typeof(string), true)]
   [InlineData(typeof(object), false)]
   [InlineData(typeof(int), false)]
   public void RequirementsAcceptOnlyString(Type type, bool expected)
      => Requirements().IsValidType(type).ShouldBe(expected);

   [Fact]
   public void ApplyUsesTheEmailAddressSource() {
      var context = new Mock<ITypePropertyConventionContext>();
      _convention.Apply(context.Object);
      context.Verify(x => x.SetSource<EmailAddressSource>(), Times.Once());
      context.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
   }
}
