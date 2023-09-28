using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class AdhocEngineConfigurationProvider(IEnumerable<Type> types) : IEngineConfigurationProvider {
   private readonly IEnumerable<IEngineConfigurationTypeProvider> _types = types.Select(x => new AdhocEngineTypeProvider(x)).ToArray();

   public IEnumerable<IEngineConfigurationTypeProvider> GetConfigurationTypes() 
      => _types;
}