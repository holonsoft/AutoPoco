using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenTypeFieldConventionApplied : ConfigurationBaseTest {

   private readonly IEngineConfiguration _configuration;
   private readonly IEngineConfigurationType _type;

   private readonly IEngineConfigurationTypeMember _testField;
   private readonly IEngineConfigurationTypeMember _testIgnoreField;

   public WhenTypeFieldConventionApplied() {
      Builder.Conventions(x => { x.Register<TestFieldConvention>(); });
      Builder.Include<TestFieldClass>()
        .Setup(x => x.Test).Default()
        .Setup(x => x.TestIgnore);

      _configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider);
      _type = _configuration.GetRegisteredType(typeof(TestFieldClass))!;
      _testField = _type.GetRegisteredMembers().Where(x => x.Member.Name == "Test").Single();
      _testIgnoreField = _type.GetRegisteredMembers().Where(x => x.Member.Name == "TestIgnore").Single();
   }

   [Fact]
   public void FieldSourceIsSetFromConvention() {
      var source = _testField.GetDataSources().First().Build()!;
      source.GetType().Should().Be(typeof(TestDataSource));
   }

   [Fact]
   public void IgnoredFieldSourceIsNotSetFromConvention() {
      var source = _testIgnoreField.GetDataSources().SingleOrDefault();
      source.Should().BeNull();
   }

   public class TestFieldClass {
      public required string Test;
      public required string TestIgnore;
   }

   public class TestFieldConvention : ITypeFieldConvention {
      public void Apply(ITypeFieldConventionContext context) => context.SetSource<TestDataSource>();

      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Name(x => x == "Test");
   }

   public class TestDataSource : IDataSource {
      public object? InternalNext(IGenerationContext? context) => null;
   }
}