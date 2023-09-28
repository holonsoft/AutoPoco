using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class GenerationContextTests {

   public GenerationContextTests() {
      IEngineConfiguration configuration = new EngineConfiguration();
      var conventionProvider = new Mock<IEngineConventionProvider>().Object;
      var repository = new GenerationConfiguration(configuration, conventionProvider, 10);
      configuration.RegisterType(typeof(SimpleUser));
      _generationSession = new GenerationContext(repository);
   }

   private readonly GenerationContext _generationSession;

   [Fact]
   public void SingleValidTypeReturnsObject() {
      var userGenerator = _generationSession.Single<SimpleUser>();
      userGenerator.Should().NotBeNull();
   }

   [Fact]
   public void SingleUnknownTypeReturnsObject() {
      var userGenerator = _generationSession.Single<SimpleUser>();
      userGenerator.Should().NotBeNull();
   }

   [Fact]
   public void ListValidTypeReturnsCollectionContext() {
      var userGenerator = _generationSession.List<SimpleUser>(10);
      userGenerator.Should().NotBeNull();
   }

   [Fact]
   public void ListUnknownTypeReturnsObjectGenerator() {
      var userGenerator = _generationSession.List<SimpleUser>(10);
      userGenerator.Should().NotBeNull();
   }

   [Fact]
   public void SinglePassesContextThroughToSession() {
      var builder = new Mock<IObjectBuilder>();
      var builderRepository = new Mock<IGenerationConfiguration>();
      builderRepository.Setup(x => x.GetBuilderForType(typeof(object))).Returns(builder.Object);
      IGenerationContext context = new GenerationContext(builderRepository.Object);

      context.Single<object>().Get();

      builder.Verify(x => x.CreateObject(context), Times.Once());
   }
}