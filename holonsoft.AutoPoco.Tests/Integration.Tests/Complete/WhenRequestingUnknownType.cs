using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenRequestingUnknownType {

   public WhenRequestingUnknownType() => _generationSession = AutoPocoContainer.CreateDefaultSession();

   private readonly IGenerationSession _generationSession;

   [Fact]
   public void WithBasicTypeValidObjectIsReturned() {
      var user = _generationSession.Single<SimpleUser>().Get();
      user.ShouldNotBeNull();
   }

   [Fact]
   public void WithDerivedTypeBasePropertiesAreFilled() {
      var obj = _generationSession.Single<SimpleDerivedClass>().Get();
      obj.BaseProperty.ShouldNotBeNull();
   }

   [Fact]
   public void WithImplementedTypeInterfacePropertiesAreFilled() {
      var obj = _generationSession.Single<SimpleDerivedClass>().Get();
      obj.InterfaceValue.ShouldNotBeNull();
   }
}