using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class NullableAnnotationSettingsTests {
   [Fact]
   public void DisabledIsOff() {
      NullableAnnotationSettings.Disabled.RespectNullableAnnotations.ShouldBeFalse();
      NullableAnnotationSettings.Disabled.NullCreationThreshold.ShouldBe(0);
   }

   [Fact]
   public void EnabledWithoutThresholdUsesTheGlobalDefault() {
      var settings = NullableAnnotationSettings.Enabled();

      settings.RespectNullableAnnotations.ShouldBeTrue();
      settings.NullCreationThreshold.ShouldBe(AutoPocoDefaults.NullCreationThreshold);
   }

   [Fact]
   public void EnabledWithThresholdKeepsIt() {
      var settings = NullableAnnotationSettings.Enabled(37);

      settings.RespectNullableAnnotations.ShouldBeTrue();
      settings.NullCreationThreshold.ShouldBe(37);
   }

   [Theory]
   [InlineData(-1)]
   [InlineData(101)]
   public void ThresholdOutsideOfPercentRangeThrows(int threshold)
      => Should.Throw<ArgumentOutOfRangeException>(() => new NullableAnnotationSettings(true, threshold));

   [Theory]
   [InlineData(0)]
   [InlineData(100)]
   public void ThresholdBoundsAreAccepted(int threshold)
      => new NullableAnnotationSettings(true, threshold).NullCreationThreshold.ShouldBe(threshold);

   [Fact]
   public void ConfigurationBuilderStartsDisabled()
      => new EngineConfigurationBuilder().NullableAnnotations.ShouldBeSameAs(NullableAnnotationSettings.Disabled);

   [Fact]
   public void ConfigurationBuilderRespectNullableAnnotationsEnablesTheSettings() {
      var builder = new EngineConfigurationBuilder();

      builder.RespectNullableAnnotations(25);

      builder.NullableAnnotations.RespectNullableAnnotations.ShouldBeTrue();
      builder.NullableAnnotations.NullCreationThreshold.ShouldBe(25);
   }

   [Fact]
   public void ConfigurationBuilderRespectNullableAnnotationsWithoutThresholdUsesTheGlobalDefault() {
      var builder = new EngineConfigurationBuilder();

      builder.RespectNullableAnnotations();

      builder.NullableAnnotations.NullCreationThreshold.ShouldBe(AutoPocoDefaults.NullCreationThreshold);
   }

   [Fact]
   public void GenerationConfigurationDefaultsToDisabled() {
      var configuration = new GenerationConfiguration(new EngineConfiguration(), new EngineConventionConfiguration(), 5);

      configuration.NullableAnnotations.ShouldBeSameAs(NullableAnnotationSettings.Disabled);
   }

   [Fact]
   public void GenerationSessionFactoryPassesTheSettingsToTheSession() {
      var settings = NullableAnnotationSettings.Enabled(60);
      IEngineConventionProvider conventionProvider = new EngineConventionConfiguration();
      var factory = new GenerationSessionFactory(new EngineConfiguration(), conventionProvider, settings);

      var session = (IGenerationContext) factory.CreateSession();

      session.Builders.NullableAnnotations.ShouldBeSameAs(settings);
   }

   [Fact]
   public void GenerationSessionFactoryWithoutSettingsIsDisabled() {
      IEngineConventionProvider conventionProvider = new EngineConventionConfiguration();
      var factory = new GenerationSessionFactory(new EngineConfiguration(), conventionProvider);

      var session = (IGenerationContext) factory.CreateSession();

      session.Builders.NullableAnnotations.ShouldBeSameAs(NullableAnnotationSettings.Disabled);
   }

   [Fact]
   public void ContainerRejectsAThresholdOutsideOfPercentRange()
      => Should.Throw<ArgumentOutOfRangeException>(() => AutoPocoContainer.Configure(x => x.RespectNullableAnnotations(101)));
}
