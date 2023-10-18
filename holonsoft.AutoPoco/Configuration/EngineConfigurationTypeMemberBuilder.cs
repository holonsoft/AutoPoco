using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeMemberBuilder(EngineTypeMember? member, EngineConfigurationTypeBuilder parentConfiguration) 
   : IEngineConfigurationTypeMemberBuilder, IEngineConfigurationTypeMemberProvider {
   private readonly List<AutoPocoDataSourceFactory> _dataSources = new();

   public IEngineConfigurationTypeBuilder Use(Type dataSource) => Use(dataSource, Array.Empty<object>());

   public IEngineConfigurationTypeBuilder Use(Type dataSource, params object[] args) {
      if (dataSource.GetInterface(typeof(IDataSource).FullName ?? throw new InvalidOperationException()) == null)
         throw new ArgumentException(@"dataSource does not implement IDataSource", nameof(dataSource));
      _dataSources.Clear();

      var newFactory = new AutoPocoDataSourceFactory(dataSource);
      newFactory.SetParams(args);
      _dataSources.Add(newFactory);
      return parentConfiguration;
   }

   public IEngineConfigurationTypeBuilder Default() {
      _dataSources.Clear();
      return parentConfiguration;
   }

   public EngineTypeMember? GetConfigurationMember() 
      => member;

   public IEnumerable<IEngineConfigurationDataSource> GetDataSources() 
      => _dataSources;

   public void SetDataSources(params AutoPocoDataSourceFactory[] dataSources) {
      _dataSources.Clear();
      if (dataSources.Length > 0)
         _dataSources.AddRange(dataSources);
   }
}