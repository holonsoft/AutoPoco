namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationProvider {
   /// <summary>
   ///   Gets the configuration types from the provider
   /// </summary>
   /// <returns></returns>
   IEnumerable<IEngineConfigurationTypeProvider> GetConfigurationTypes();
}