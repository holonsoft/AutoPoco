using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConventionConfigurationTests {
   [Fact]
   public void RegisterAddsConvention() {
      var config = new EngineConventionConfiguration();
      config.Register(typeof(SimpleMemberConvention));

      var addedType = config.Find<SimpleMemberConvention>().Single();
      addedType.Should().BeSameAs(typeof(SimpleMemberConvention));

      //Assert.AreEqual(typeof(SimpleMemberConvention), addedType);
   }

   [Fact]
   public void RegisterGenericAddsConvention() {
      var config = new EngineConventionConfiguration();
      config.Register<SimpleMemberConvention>();

      var addedType = config.Find<SimpleMemberConvention>().Single();
      addedType.Should().BeSameAs(typeof(SimpleMemberConvention));

      //Assert.AreEqual(typeof(SimpleMemberConvention), addedType);
   }

   [Fact]
   public void UseDefaultConventionsAddsDefaultConventions() {
      var config = new EngineConventionConfiguration();
      config.UseDefaultConventions();

      var addedType = config.Find<DefaultDatetimeMemberConvention>().Single();
      addedType.Should().BeSameAs(typeof(DefaultDatetimeMemberConvention));
   }

   [Fact]
   public void ScanAssemblyWithTypeAddsAssemblyConventions() {
      var config = new EngineConventionConfiguration();
      config.ScanAssemblyWithType<SimpleMemberConvention>();

      var addedType = config.Find<SimpleTypeConvention>().Single();
      addedType.Should().BeSameAs(typeof(SimpleTypeConvention));
   }

   [Fact]
   public void ScanAssemblyAddsAssemblyConvention() {
      var config = new EngineConventionConfiguration();
      config.ScanAssembly(typeof(SimpleMemberConvention).Assembly);

      var addedType = config.Find<SimpleTypeConvention>().Single();
      addedType.Should().BeSameAs(typeof(SimpleTypeConvention));
   }

   [Fact]
   public void FindReturnsAllConventions() {
      var config = new EngineConventionConfiguration();
      config.ScanAssembly(typeof(SimpleMemberConvention).Assembly);

      var conventionTypes = config.Find<IConvention>().ToArray();
      conventionTypes.Length.Should().Be(16);
   }
}