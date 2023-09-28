using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class GenerationSessionFactory(IEngineConfiguration config, IEngineConventionProvider conventionProvider) : IGenerationSessionFactory {
   public IGenerationSession CreateSession(int recursionLimit) 
      => new GenerationContext(new GenerationConfiguration(config, conventionProvider, recursionLimit));

   public IGenerationSession CreateSession() =>
      // TODO: Need to deep-clone the config
      CreateSession(5);
}