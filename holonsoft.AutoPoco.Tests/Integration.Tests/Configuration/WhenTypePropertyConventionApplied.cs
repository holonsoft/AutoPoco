using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenTypePropertyConventionApplied : ConfigurationBaseTest {

   public WhenTypePropertyConventionApplied() {
      Builder.Conventions(x => { x.Register<TestPropertyConvention>(); });
      Builder.Include<TestPropertyClass>()
        .Setup(x => x.Test).Default()
        .Setup(x => x.TestIgnore);

      _configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider);
      _type = _configuration.GetRegisteredType(typeof(TestPropertyClass))!;
      _testProperty = _type.GetRegisteredMembers().Where(x => x.Member.Name == "Test").Single();
      _testIgnoreProperty = _type.GetRegisteredMembers().Where(x => x.Member.Name == "TestIgnore").Single();
   }

   private readonly IEngineConfiguration _configuration;
   private readonly IEngineConfigurationType _type;
   private readonly IEngineConfigurationTypeMember _testProperty;
   private readonly IEngineConfigurationTypeMember _testIgnoreProperty;

   [Fact]
   public void TestPropertySourceIsSetFromConvention() {
      var source = _testProperty.GetDataSources().First().Build()!;
      source.GetType().Should().Be(typeof(TestDataSource));
   }

   [Fact]
   public void IgnoredPropertySourceIsNotSetFromConvention() {
      var source = _testIgnoreProperty.GetDataSources().SingleOrDefault();
      source.Should().BeNull();
   }

   public class TestPropertyClass {
      public required string Test { get; set; }

      public required string TestIgnore { get; set; }
   }

   public class TestPropertyConvention : ITypePropertyConvention {
      public void Apply(ITypePropertyConventionContext context) => context.SetSource<TestDataSource>();

      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Name(x => x == "Test");
   }

   public class TestDataSource : IDataSource {
      public object? InternalNext(IGenerationContext? context) => null;
   }
}