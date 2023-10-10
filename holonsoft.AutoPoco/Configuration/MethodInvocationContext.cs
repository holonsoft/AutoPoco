using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Configuration;

public class MethodInvocationContext {
   private readonly List<DataSourceFactory> _arguments = new();

   public void AddArgumentSource(Type source, params object[]? args) {
      var factory = new DataSourceFactory(source);
      factory.SetParams(args!);
      _arguments.Add(factory);
   }

   public void AddArgumentSource(Type source)
      => AddArgumentSource(source, null);

   public void AddArgumentValue(object value)
      => AddArgumentSource(typeof(ValueSource<object>), value);

   public IEnumerable<DataSourceFactory> GetArguments()
      => _arguments;
}