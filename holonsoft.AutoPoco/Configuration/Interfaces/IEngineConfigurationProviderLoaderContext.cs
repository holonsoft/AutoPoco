namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationProviderLoaderContext {
   IEngineConfiguration Configuration { get; }
   IEngineConfigurationProvider ConfigurationProvider { get; }
   IEngineConventionProvider ConventionProvider { get; }
}