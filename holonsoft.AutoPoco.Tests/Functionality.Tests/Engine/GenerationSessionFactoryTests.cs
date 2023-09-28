using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class GenerationSessionFactoryTests {
   [Fact]
   public void CreateSessionEmptyConfigReturnsSession() {
      IEngineConventionProvider conventionProvider = new EngineConventionConfiguration();
      var config = new GenerationSessionFactory(new EngineConfiguration(), conventionProvider);
      var session = config.CreateSession();
      session.Should().NotBeNull();
   }
}