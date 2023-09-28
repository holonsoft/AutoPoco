using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class GenerationConfiguration(IEngineConfiguration configuration, IEngineConventionProvider conventionProvider,
  int recursionLimit) : IGenerationConfiguration {
   private readonly List<IObjectBuilder> _objectBuilders = new();

   public int RecursionLimit { get; } = recursionLimit;

   public IObjectBuilder GetBuilderForType(Type searchType) {
      var builder = _objectBuilders.SingleOrDefault(x => x.InnerType == searchType);
      builder ??= CreateBuilderForType(searchType);
      return builder;
   }

   private IObjectBuilder CreateBuilderForType(Type searchType) {
      EnsureTypeExists(searchType);
      var type = configuration.GetRegisteredType(searchType);
      var builder = new ObjectBuilder(type!);
      _objectBuilders.Add(builder);
      return builder;
   }

   private void EnsureTypeExists(Type searchType) {
      if (configuration.GetRegisteredType(searchType) != null)
         return;

      var provider = new AdhocEngineConfigurationProvider(new[] { searchType });
      var coreConvention = new DefaultEngineConfigurationProviderLoadingConvention();
      coreConvention.Apply(new EngineConfigurationProviderLoaderContext(configuration, provider, conventionProvider));
   }
}