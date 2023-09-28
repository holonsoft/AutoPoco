using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenThreeTypesAreRegistered : ConfigurationBaseTest {

   public WhenThreeTypesAreRegistered() {
      Builder.Include<SimpleUser>();
      Builder.Include<SimplePropertyClass>();
      Builder.Include<SimpleFieldClass>();
      Configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider);
   }

   protected IEngineConfiguration Configuration { get; private set; }

   [Fact]
   public void ConfigurationContainsFourTypes() {
      var types = Configuration.GetRegisteredTypes();
      types.Count().Should().Be(4);
   }

   [Fact]
   public void ConfigurationContainsValidTypesIncludingSystemObject() {
      var simpleUserType = Configuration.GetRegisteredType(typeof(SimpleUser));
      var simplePropertyType = Configuration.GetRegisteredType(typeof(SimplePropertyClass));
      var simpleFieldType = Configuration.GetRegisteredType(typeof(SimpleFieldClass));
      var objectType = Configuration.GetRegisteredType(typeof(object));

      simpleUserType.Should().NotBeNull();
      simplePropertyType.Should().NotBeNull();
      simpleFieldType.Should().NotBeNull();
      objectType.Should().NotBeNull();
   }
}