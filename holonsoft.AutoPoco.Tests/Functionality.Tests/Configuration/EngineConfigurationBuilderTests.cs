using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationBuilderTests {
   [Fact]
   public void GenericIncludeReturnsTypeConfiguration() {
      var config = new EngineConfigurationBuilder();
      var user = config.Include<SimpleUser>();

      Assert.NotNull(user);
   }

   [Fact]
   public void NonGenericIncludeReturnsTypeConfiguration() {
      var config = new EngineConfigurationBuilder();
      var builder = config.Include(typeof(SimpleUser));

      Assert.NotNull(builder);
   }

   [Fact]
   public void ConventionsInvokesAction() {
      var config = new EngineConfigurationBuilder();
      var wasInvoked = false;
      config.Conventions(x => { wasInvoked = true; });

      wasInvoked.Should().BeTrue();
   }

   [Fact]
   public void RegisterTypeProviderRegistersTypeProvider() {
      var config = new EngineConfigurationBuilder();
      var providerMock = new Mock<IEngineConfigurationTypeProvider>();
      config.RegisterTypeProvider(providerMock.Object);

      config.GetConfigurationTypes().Contains(providerMock.Object).Should().BeTrue();
   }
}