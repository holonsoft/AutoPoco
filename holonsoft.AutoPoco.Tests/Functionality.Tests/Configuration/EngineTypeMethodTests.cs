using System.Reflection;
using Xunit;
using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineTypeMethodTests {
   [Fact]
   public void SameMethodRequestedFromBaseAndDerivedTypesIsConsideredEqual() {
      var baseMethod = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("SealedMethod", Type.EmptyTypes)!);
      var derivedMethod = new EngineTypeMethodMember(typeof(DerivedClass).GetMethod("SealedMethod", Type.EmptyTypes)!);

      Assert.True(baseMethod == derivedMethod);
   }

   [Fact]
   public void SameMethodRequestedFromInterfaceAndImplementingTypeIsConsideredEqual() {
      var interfaceMethod = new EngineTypeMethodMember(typeof(IFoo).GetMethod("InterfaceMethod", Type.EmptyTypes)!);
      var implementedMethod = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("InterfaceMethod", Type.EmptyTypes)!);

      Assert.True(interfaceMethod == implementedMethod);
   }

   [Fact]
   public void SameMethodRequestedFromInterfaceAndDerivedTypeFromImplementingTypeIsConsideredEqual() {
      var interfaceMethod = new EngineTypeMethodMember(typeof(IFoo).GetMethod("InterfaceMethod", Type.EmptyTypes)!);
      var deriveddMethod = new EngineTypeMethodMember(typeof(DerivedClass).GetMethod("InterfaceMethod", Type.EmptyTypes)!);

      Assert.True(interfaceMethod == deriveddMethod);
   }

   [Fact]
   public void OverriddenMethodRequestedFromBaseAndDerivedTypeIsConsideredEqual() {
      var baseMethod = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("VirtualMethod", Type.EmptyTypes)!);
      var overriddenMethod = new EngineTypeMethodMember(typeof(DerivedClass).GetMethod("VirtualMethod", Type.EmptyTypes)!);

      Assert.True(baseMethod == overriddenMethod);
   }

#pragma warning disable xUnit1004 // Test methods should not be skipped
   [Fact(Skip = "Not important yet")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
   public void NewMethodRequestedFromDerivedTypeIsNotConsideredToHiddenMethod() {
      var flags = BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance |
                  BindingFlags.Public | BindingFlags.Static;

      var baseMethod = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("AnotherSealedMethod", flags, null, Type.EmptyTypes, null)!);
      var overriddenMethod = new EngineTypeMethodMember(typeof(DerivedClass).GetMethod("AnotherSealedMethod", flags, null, Type.EmptyTypes, null)!);

      Assert.False(baseMethod == overriddenMethod);
   }

   [Fact]
   public void DifferentMethodAreNotConsideredEqual() {
      var methodOne = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("SealedMethod", Type.EmptyTypes)!);
      var methodTwo = new EngineTypeMethodMember(typeof(BaseClass).GetMethod("SealedMethod", new[] { typeof(int) })!);

      Assert.False(methodOne == methodTwo);
   }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

   public interface IFoo {
      void InterfaceMethod();
   }

   public class BaseClass : IFoo {
      public void InterfaceMethod() {
      }

      public void SealedMethod() {
      }

      public void AnotherSealedMethod() {
      }

      public virtual void VirtualMethod() {
      }

      public void SealedMethod(int param) {
      }
   }

   public class DerivedClass : BaseClass {
      public override void VirtualMethod() {
      }

      public new void AnotherSealedMethod() {
      }
   }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static

}