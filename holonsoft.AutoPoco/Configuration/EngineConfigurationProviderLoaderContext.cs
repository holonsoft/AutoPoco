using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationProviderLoaderContext(
  IEngineConfiguration configuration,
  IEngineConfigurationProvider configurationProvider,
  IEngineConventionProvider conventionProvider) : IEngineConfigurationProviderLoaderContext {
   public IEngineConfiguration Configuration { get; } = configuration;

   public IEngineConfigurationProvider ConfigurationProvider { get; } = configurationProvider;

   public IEngineConventionProvider ConventionProvider { get; } = conventionProvider;
}