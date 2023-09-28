using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Engine;

public class GenerationSessionFactoryTestBase {

   public GenerationSessionFactoryTestBase() {
      Configuration = new EngineConfiguration();
      IEngineConventionProvider conventionProvider = new EngineConventionConfiguration();
      PopulateConfiguration();
      var factory = new GenerationSessionFactory(
        Configuration, conventionProvider);
      GenerationSession = factory.CreateSession();
   }

   public EngineConfiguration Configuration { get; private set; }

   public IGenerationSession GenerationSession { get; private set; }

   protected virtual void PopulateConfiguration() {
   }
}