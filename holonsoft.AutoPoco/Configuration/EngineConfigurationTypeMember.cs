using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeMember(EngineTypeMember member) : IEngineConfigurationTypeMember {
   private readonly List<IEngineConfigurationDataSource> _dataSources = new();

   public EngineTypeMember Member { get; } = member;

   public void SetDataSource(IEngineConfigurationDataSource action) {
      _dataSources.Clear();
      _dataSources.Add(action);
   }

   public void SetDataSources(IEnumerable<IEngineConfigurationDataSource> sources) {
      _dataSources.Clear();
      _dataSources.AddRange(sources);
   }

   public IEnumerable<IEngineConfigurationDataSource> GetDataSources() 
      => _dataSources.ToArray();
}