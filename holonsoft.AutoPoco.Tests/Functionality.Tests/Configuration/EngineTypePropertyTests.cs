using Shouldly;
using System.Reflection;
using Xunit;
using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineTypePropertyTests {
   [Fact]
   public void SamePropertyRequestedFromBaseAndDerivedTypesIsConsideredEqual() {
      var baseProperty = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("SealedProperty")!);
      var derivedProperty = new EngineTypePropertyMember(typeof(DerivedClass).GetProperty("SealedProperty")!);
      (baseProperty == derivedProperty).ShouldBeTrue();
      baseProperty.ShouldBe(derivedProperty);
   }

   [Fact]
   public void SamePropertyRequestedFromInterfaceAndImplementingTypeIsConsideredEqual() {
      var interfaceProperty = new EngineTypePropertyMember(typeof(IFoo).GetProperty("InterfaceProperty")!);
      var implementedProperty = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("InterfaceProperty")!);

      (interfaceProperty == implementedProperty).ShouldBeTrue();
   }

   [Fact]
   public void SamePropertyRequestedFromInterfaceAndDerivedTypeFromImplementingTypeIsConsideredEqual() {
      var interfaceProperty = new EngineTypePropertyMember(typeof(IFoo).GetProperty("InterfaceProperty")!);
      var derivedProperty = new EngineTypePropertyMember(typeof(DerivedClass).GetProperty("InterfaceProperty")!);
      interfaceProperty.ShouldBe(derivedProperty);
      //Assert.True(interfaceProperty == deriveddProperty);
   }

   [Fact]
   public void OverriddenPropertyRequestedFromBaseAndDerivedTypeIsConsideredEqual() {
      var baseProperty = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("VirtualProperty")!);
      var overriddenProperty = new EngineTypePropertyMember(typeof(DerivedClass).GetProperty("VirtualProperty")!);
      overriddenProperty.ShouldBe(baseProperty);
      //Assert.True(baseProperty == overriddenProperty);
   }

#pragma warning disable xUnit1004 // Test methods should not be skipped
   [Fact(Skip = "Not important yet (Nobody has complained)")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
   public void NewPropertyRequestedFromDerivedTypeIsNotConsideredToHiddenProperty() {
      var flags = BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance |
                  BindingFlags.Public | BindingFlags.Static;

      var baseProperty = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("AnotherSealedProperty", flags)!);
      var overriddenProperty = new EngineTypePropertyMember(typeof(DerivedClass).GetProperty("AnotherSealedProperty", flags)!);
      overriddenProperty.ShouldNotBe(baseProperty);

      //Assert.False(baseProperty == overriddenProperty);
   }

   [Fact]
   public void DifferentPropertiesAreNotConsideredEqual() {
      var propertyOne = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("SealedProperty")!);
      var propertyTwo = new EngineTypePropertyMember(typeof(BaseClass).GetProperty("AnotherSealedProperty")!);

      propertyOne.ShouldNotBe(propertyTwo);
   }

   public interface IFoo {
      string InterfaceProperty { get; set; }
   }

   public class BaseClass : IFoo {
      public required string SealedProperty { get; set; }
      public string? AnotherSealedProperty { get; set; }
      public virtual required string VirtualProperty { get; set; }
      public required string InterfaceProperty { get; set; }
   }

   public class DerivedClass : BaseClass {
      public override required string VirtualProperty { get; set; }
      public new string? AnotherSealedProperty { get; set; }
   }
}