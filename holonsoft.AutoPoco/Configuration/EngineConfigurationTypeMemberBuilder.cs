using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeMemberBuilder : IEngineConfigurationTypeMemberBuilder, IEngineConfigurationTypeMemberProvider {
   private readonly EngineTypeMember? _member;
   private readonly EngineConfigurationTypeBuilder _parentConfiguration;
   private readonly List<AutoPocoDataSourceFactory> _dataSources = new();

   public EngineConfigurationTypeMemberBuilder(EngineTypeMember? member, EngineConfigurationTypeBuilder parentConfiguration) {
      ArgumentNullException.ThrowIfNull(parentConfiguration);
      _member = member;
      _parentConfiguration = parentConfiguration;
   }

   public IEngineConfigurationTypeBuilder Use(Type dataSource) => Use(dataSource, Array.Empty<object>());

   public IEngineConfigurationTypeBuilder Use(Type dataSource, params object[] args) {
      ArgumentNullException.ThrowIfNull(dataSource);
      ArgumentNullException.ThrowIfNull(args);
      if (!typeof(IDataSource).IsAssignableFrom(dataSource))
         throw new ArgumentException($"'{dataSource.FullName}' does not implement IDataSource", nameof(dataSource));
      _dataSources.Clear();

      var newFactory = new AutoPocoDataSourceFactory(dataSource);
      newFactory.SetParams(args);
      _dataSources.Add(newFactory);
      return _parentConfiguration;
   }

   public IEngineConfigurationTypeBuilder Default() {
      _dataSources.Clear();
      return _parentConfiguration;
   }

   public EngineTypeMember? GetConfigurationMember()
      => _member;

   public IEnumerable<IEngineConfigurationDataSource> GetDataSources()
      => _dataSources;

   public void SetDataSources(params AutoPocoDataSourceFactory[] dataSources) {
      ArgumentNullException.ThrowIfNull(dataSources);
      _dataSources.Clear();
      if (dataSources.Length > 0)
         _dataSources.AddRange(dataSources);
   }
}
