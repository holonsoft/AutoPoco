using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenCreatedTypeHasBaseTypes {

   public WhenCreatedTypeHasBaseTypes()
      => _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.Include<ISimpleInterface>()
           .Setup(c => c.InterfaceValue).Value("Interface")
           .Setup(c => c.OtherInterfaceValue).Value("Interface");
         x.Include<SimpleBaseClass>()
           .Setup(c => c.BaseProperty).Value("Test")
           .Setup(c => c.BaseVirtualProperty).Value("Base");
         x.Include<SimpleDerivedClass>()
           .Setup(c => c.Name).Value("OtherTest")
           .Setup(c => c.BaseVirtualProperty).Value("Derived")
           .Setup(c => c.OtherInterfaceValue).Value("Derived")
           .Setup(c => c.Name).Use<FirstNameSource>();
      })
   .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void DerivedTypeHasInterfaceValue() {
      var derivedClass = _session.Single<SimpleDerivedClass>().Get();
      derivedClass.InterfaceValue.ShouldBe("Interface");
   }

   [Fact]
   public void DerivedTypeHasOverrideInterfaceValue() {
      var derivedClass = _session.Single<SimpleDerivedClass>().Get();
      derivedClass.OtherInterfaceValue.ShouldBe("Derived");
   }

   [Fact]
   public void DerivedTypeOverriddenMemberHasDerivedValue() {
      var derivedClass = _session.Single<SimpleDerivedClass>().Get();
      derivedClass.BaseVirtualProperty.ShouldBe("Derived");
   }

   [Fact]
   public void DerivedTypeContainsBaseValues() {
      var derivedClass = _session.Single<SimpleDerivedClass>().Get();
      derivedClass.BaseProperty.ShouldBe("Test");
   }
}