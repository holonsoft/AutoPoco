using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Extensions;

public static class ConfigurationExtensions {
   extension(IEngineConfigurationBuilder builder) {
      /// <summary>
      ///   Includes every type of the assembly that contains <typeparamref name="T" />.
      /// </summary>
      public IEngineConfigurationBuilder AddFromAssemblyContainingType<T>() {
         foreach (var type in typeof(T).Assembly.GetTypes())
            builder.Include(type);
         return builder;
      }
   }
}
