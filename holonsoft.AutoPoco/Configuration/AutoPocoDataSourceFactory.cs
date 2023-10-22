using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class AutoPocoDataSourceFactory(Type t) : IEngineConfigurationDataSource {
   private object[]? _params;
   private Action<object>? _action;
   private object? _userDefinedFactory;

   public IDataSource? Build() {
      if (_userDefinedFactory != null) {
         return (IDataSource) _userDefinedFactory
            .GetType()
            .InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, _userDefinedFactory, null)!;
      }

      var instance = Activator.CreateInstance(t, _params);

      _action?.Invoke(instance!);

      return instance as IDataSource;
   }

   public void SetParams(params object[] args)
      => _params = args;
   public void SetAction(Action<object> action)
      => _action = action;
   public void SetUserFactory<TMember>(IDataSourceFactory<TMember> userDefinedFactory)
      => _userDefinedFactory = userDefinedFactory;
}