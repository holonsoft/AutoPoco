using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class TypeMemberConventionContext(IEngineConfiguration configuration, IEngineConfigurationTypeMember member) {
   public IEngineConfiguration Configuration { get; } = configuration;

   public EngineTypeMember Member => member.Member;

   public void SetValue(object value) {
      var type = typeof(ValueSource<>).MakeGenericType(value.GetType());
      var factory = new DataSourceFactory(type);
      factory.SetParams(value);
      member.SetDataSource(factory);
   }

   public void SetSource<T>() where T : IDataSource 
      => SetSource(typeof(T));

   public void SetSource(Type t) 
      => member.SetDataSource(new DataSourceFactory(t));
}