using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class AdhocEngineTypeProvider(Type type) : IEngineConfigurationTypeProvider {
   public Type GetConfigurationType() => type;

   public IEnumerable<IEngineConfigurationTypeMemberProvider> GetConfigurationMembers() 
      => Array.Empty<IEngineConfigurationTypeMemberProvider>();

   public IEngineConfigurationDataSource? GetFactory() 
      => null;
}