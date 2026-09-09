using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationBuilder : IEngineConfigurationBuilder, IEngineConfigurationProvider {
   private readonly EngineConventionConfiguration _conventions = new();
   private readonly List<IEngineConfigurationTypeProvider> _types = new();

   public IEngineConventionProvider ConventionProvider => _conventions;

   public NullableAnnotationSettings NullableAnnotations { get; private set; } = NullableAnnotationSettings.Disabled;

   public int Seed { get; private set; } = AutoPocoDefaults.Seed;

   public IEngineConfigurationTypeBuilder<T> Include<T>() {
      // Create the configuration
      var configuration = new EngineConfigurationTypeBuilder<T>();

      // Store it locally
      _types.Add(configuration);

      //And return the public interface
      return configuration;
   }

   public IEngineConfigurationTypeBuilder Include(Type t) {
      ArgumentNullException.ThrowIfNull(t);

      // Create the configuration
      var configuration = new EngineConfigurationTypeBuilder(t);

      // Store it locally
      _types.Add(configuration);

      //And return the public interface
      return configuration;
   }

   public void Conventions(Action<IEngineConventionConfiguration> config) {
      ArgumentNullException.ThrowIfNull(config);
      config.Invoke(_conventions);
   }

   public void RegisterTypeProvider(IEngineConfigurationTypeProvider provider) {
      ArgumentNullException.ThrowIfNull(provider);
      _types.Add(provider);
   }

   public IEnumerable<IEngineConfigurationTypeProvider> GetConfigurationTypes()
      => _types;

   public void RespectNullableAnnotations(int? nullCreationThreshold = null)
      => NullableAnnotations = NullableAnnotationSettings.Enabled(nullCreationThreshold);

   public void UseSeed(int seed)
      => Seed = seed;
}
