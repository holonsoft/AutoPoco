using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Extensions;

public static class ConfigurationExtensions {
   public static IEngineConfigurationBuilder AddFromAssemblyContainingType<T>(this IEngineConfigurationBuilder builder) {
      foreach (var type in typeof(T).Assembly.GetTypes())
         builder.Include(type);
      return builder;
   }
}