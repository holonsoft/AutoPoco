using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class DataSourceFactory(Type t) : IEngineConfigurationDataSource {
   private object[]? _params;

   public IDataSource? Build() 
      => Activator.CreateInstance(t, _params) as IDataSource;

   public void SetParams(params object[] args) 
      => _params = args;
}