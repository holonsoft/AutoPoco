using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Configuration;

/// <summary>
///   The data sources feeding the arguments of a method invoked on every generated object, in parameter order.
/// </summary>
public class MethodInvocationContext {
   private readonly List<AutoPocoDataSourceFactory> _arguments = new();

   /// <summary>
   ///   The argument comes from a data source of the given type, created with the given constructor arguments.
   /// </summary>
   public void AddArgumentSource(Type source, params object[]? args) {
      ArgumentNullException.ThrowIfNull(source);
      var factory = new AutoPocoDataSourceFactory(source);
      factory.SetParams(args!);
      _arguments.Add(factory);
   }

   public void AddArgumentSource(Type source)
      => AddArgumentSource(source, null);

   /// <summary>
   ///   The argument is a fixed value, null included.
   /// </summary>
   public void AddArgumentValue(object? value) {
      if (value is null)
         AddArgumentSource(typeof(FuncSource<object?>), new Func<object?>(() => null));
      else
         AddArgumentSource(typeof(ValueSource<object>), value);
   }

   public IEnumerable<AutoPocoDataSourceFactory> GetArguments()
      => _arguments;
}
