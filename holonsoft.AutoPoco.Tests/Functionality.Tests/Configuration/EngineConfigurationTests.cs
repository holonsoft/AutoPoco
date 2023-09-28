using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationTests {
   [Fact]
   public void RegisterTypeTypeAlreadyRegisteredThrowsException() {
      var configuration = new EngineConfiguration();
      configuration.RegisterType(typeof(SimpleUser));

      Assert.Throws<ArgumentException>(() => { configuration.RegisterType(typeof(SimpleUser)); });
   }
}