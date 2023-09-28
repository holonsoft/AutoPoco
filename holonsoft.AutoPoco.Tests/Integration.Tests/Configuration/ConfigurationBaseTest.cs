using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class ConfigurationBaseTest {
   public EngineConfigurationBuilder Builder { get; private set; }

   public ConfigurationBaseTest()
      => Builder = new EngineConfigurationBuilder();
}