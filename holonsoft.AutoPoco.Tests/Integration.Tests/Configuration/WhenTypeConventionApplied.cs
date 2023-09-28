using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenTypeConventionApplied : ConfigurationBaseTest {

   public WhenTypeConventionApplied() {
      Builder.Conventions(x => { x.Register<TestTypeConvention>(); });
      Builder.Include<TestTypeClass>();

      _configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider)!;
      _type = _configuration.GetRegisteredType(typeof(TestTypeClass))!;
   }

   private readonly IEngineConfiguration _configuration;
   private readonly IEngineConfigurationType _type;

   [Fact]
   public void ConfigurationContainsMemberOnRegisteredType()
      => _type.GetRegisteredMembers().Where(x => x.Member.Name == "Test").Count().Should().Be(1);

   public class TestTypeClass {
      public string? Test = null!;
   }

   public class TestTypeConvention : ITypeConvention {
      public void Apply(ITypeConventionContext context) => context.RegisterField(typeof(TestTypeClass).GetField("Test")!);
   }
}