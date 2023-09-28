using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationFactory : IEngineConfigurationFactory {
   public virtual IEngineConfiguration Create(IEngineConfigurationProvider configurationProvider, IEngineConventionProvider conventionProvider) {
      var configuration = new EngineConfiguration();
      var coreConvention = new DefaultEngineConfigurationProviderLoadingConvention();
      coreConvention.Apply(new EngineConfigurationProviderLoaderContext(configuration, configurationProvider, conventionProvider));
      return configuration;
   }
}