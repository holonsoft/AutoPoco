using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConventionConfiguration : IEngineConventionConfiguration, IEngineConventionProvider {
   private readonly HashSet<Type> _conventions = new();

   public void Register(Type conventionType) => _conventions.Add(conventionType);

   public void Register<T>() where T : IConvention => Register(typeof(T));

   public void UseDefaultConventions() => ScanAssemblyWithType<EngineConventionConfiguration>();

   public void ScanAssemblyWithType<T>() => ScanAssembly(typeof(T).Assembly);

   public void ScanAssembly(Assembly assembly) {
      foreach (var type in assembly.GetTypes()
                 .Where(x => typeof(IConvention).IsAssignableFrom(x)))
         Register(type);
   }

   public IEnumerable<Type> Find<T>() where T : IConvention => _conventions.Where(x =>
        x.IsClass
        && typeof(T).IsAssignableFrom(x)
        && !x.IsAbstract);
}