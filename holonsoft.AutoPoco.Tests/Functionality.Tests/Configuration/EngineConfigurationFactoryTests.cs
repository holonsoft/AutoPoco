using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationFactoryTests {
   [Fact]
   public void CreateWithEmptySetupReturnsConfiguration() {
      var configurationProviderMock = new Mock<IEngineConfigurationProvider>();
      var conventionProviderMock = new Mock<IEngineConventionProvider>();
      var factory = new EngineConfigurationFactory();

      var configuration = factory.Create(
        configurationProviderMock.Object,
        conventionProviderMock.Object);

      configuration.Should().NotBeNull();
   }
}