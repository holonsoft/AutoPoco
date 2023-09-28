using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationBuilder : IEngineConfigurationBuilder, IEngineConfigurationProvider {
   private readonly EngineConventionConfiguration _conventions = new();
   private readonly List<IEngineConfigurationTypeProvider> _types = new();

   public IEngineConventionProvider ConventionProvider => _conventions;

   public IEngineConfigurationTypeBuilder<T> Include<T>() {
      // Create the configuration
      var configuration = new EngineConfigurationTypeBuilder<T>();

      // Store it locally
      _types.Add(configuration);

      //And return the public interface
      return configuration;
   }

   public IEngineConfigurationTypeBuilder Include(Type t) {
      // Create the configuration
      var configuration = new EngineConfigurationTypeBuilder(t);

      // Store it locally
      _types.Add(configuration);

      //And return the public interface
      return configuration;
   }

   public void Conventions(Action<IEngineConventionConfiguration> config) 
      => config.Invoke(_conventions);

   public void RegisterTypeProvider(IEngineConfigurationTypeProvider provider) 
      => _types.Add(provider);

   public IEnumerable<IEngineConfigurationTypeProvider> GetConfigurationTypes() 
      => _types;
}