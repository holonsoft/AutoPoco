namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationFactory {
   /// <summary>
   ///   Creates an engine configuration from a configuration provider and a set of conventions
   /// </summary>
   /// <returns></returns>
   IEngineConfiguration Create(IEngineConfigurationProvider configurationProvider, IEngineConventionProvider conventionProvider);
}