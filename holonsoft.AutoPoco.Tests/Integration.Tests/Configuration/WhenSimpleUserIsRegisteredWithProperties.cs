using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenSimpleUserIsRegisteredWithProperties : ConfigurationBaseTest {
   protected IEngineConfiguration Configuration { get; private set; }

   public WhenSimpleUserIsRegisteredWithProperties() {
      Builder.Include<SimpleUser>()
        .Setup(x => x.EmailAddress).Default()
        .Setup(x => x.FirstName).Default()
        .Setup(x => x.LastName).Default();

      Configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider);
   }

   [Fact]
   public void ContainsOnlyTheRegisteredProperties() {
      var type = Configuration.GetRegisteredType(typeof(SimpleUser))!;
      var members = type.GetRegisteredMembers();
      members.Count().Should().Be(3);
   }

   [Fact]
   public void ContainsEmailAddress() {
      var type = Configuration.GetRegisteredType(typeof(SimpleUser))!;
      var emailAddressProperty = type.GetRegisteredMember(ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress));
      emailAddressProperty.Should().NotBeNull();
   }

   [Fact]
   public void ContainsFirstName() {
      var type = Configuration.GetRegisteredType(typeof(SimpleUser))!;
      var firstNameProperty = type.GetRegisteredMember(ReflectionHelper.GetMember<SimpleUser>(x => x.FirstName));
      firstNameProperty.Should().NotBeNull();
   }

   [Fact]
   public void ContainsLastName() {
      var type = Configuration.GetRegisteredType(typeof(SimpleUser))!;
      var lastNameProperty = type.GetRegisteredMember(ReflectionHelper.GetMember<SimpleUser>(x => x.LastName));
      lastNameProperty.Should().NotBeNull();
   }
}