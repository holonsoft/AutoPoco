namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeMemberProvider {
   EngineTypeMember? GetConfigurationMember();

   IEnumerable<IEngineConfigurationDataSource> GetDataSources();
}