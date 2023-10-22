using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeMemberBuilder<TPoco, TMember>(EngineTypeMember member,
  EngineConfigurationTypeBuilder<TPoco> parentConfiguration) : EngineConfigurationTypeMemberBuilder(member, parentConfiguration),
  IEngineConfigurationTypeMemberBuilder<TPoco, TMember> {
   private readonly IEngineConfigurationTypeBuilder<TPoco> _parentConfiguration = parentConfiguration;

   public IEngineConfigurationTypeBuilder<TPoco> Use<TSource>()
      where TSource : IDataSource<TMember> => Use<TSource>(Array.Empty<object>());

   public IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(params object[] args) where TSource : IDataSource<TMember> {
      var factory = new AutoPocoDataSourceFactory(typeof(TSource));

      factory.SetParams(args);
      SetDataSources(factory);
      return _parentConfiguration;
   }


   public IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(Action<TSource> action)
      where TSource : IDataSource<TMember> {

      var factory = new AutoPocoDataSourceFactory(typeof(TSource));

      factory.SetAction(new Action<object>(o => action((TSource) o)));

      SetDataSources(factory);
      return _parentConfiguration;
   }


   public IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(IDataSourceFactory<TMember> userFactory) where TSource : IDataSource<TMember> {
      var factory = new AutoPocoDataSourceFactory(typeof(TSource));

      factory.SetUserFactory(userFactory);

      SetDataSources(factory);
      return _parentConfiguration;
   }

   public new IEngineConfigurationTypeBuilder<TPoco> Default() {
      base.Default();
      return _parentConfiguration;
   }
}