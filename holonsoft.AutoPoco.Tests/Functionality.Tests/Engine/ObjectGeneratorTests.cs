using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class ObjectGeneratorTests {
   public ObjectGeneratorTests() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      builder.AddAction(
        new ObjectPropertySetFromSourceAction(
          (ReflectionHelper.GetMember<SimpleUser>(x => x.FirstName) as EngineTypePropertyMember)!,
          new SimpleDataSource(_testPropertyValue)
        ));
      builder.AddAction(
        new ObjectPropertySetFromSourceAction(
          (ReflectionHelper.GetMember<SimpleUser>(x => x.LastName) as EngineTypePropertyMember)!,
          new SimpleDataSource(_testPropertyValue)
        ));
      builder.AddAction(
        new ObjectPropertySetFromSourceAction(
          (ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress) as EngineTypePropertyMember)!,
          new SimpleDataSource(_testPropertyValue)
        ));

      var builderRepository = new Mock<IGenerationConfiguration>();
      builderRepository.Setup(x => x.GetBuilderForType(typeof(SimpleUser))).Returns(builder);
      builderRepository.SetupGet(x => x.RecursionLimit).Returns(10);

      _generationContext = new GenerationContext(builderRepository.Object);
      _methodGenerator = new ObjectGenerator<SimpleMethodClass>(
        _generationContext, builder);

      _userGenerator = new ObjectGenerator<SimpleUser>(_generationContext, builder);
   }

   private readonly string _testPropertyValue = "TestValue";
   private readonly ObjectGenerator<SimpleUser> _userGenerator;
   private readonly ObjectGenerator<SimpleMethodClass> _methodGenerator;
   private readonly GenerationContext _generationContext;

   [Fact]
   public void SingleReturnsSingleObject() {
      var user = _userGenerator.Get();
      user.FirstName.Should().Be(_testPropertyValue);
      user.LastName.Should().Be(_testPropertyValue);
      user.EmailAddress.Should().Be(_testPropertyValue);
   }

   [Fact]
   public void SingleWrapsUpContextWithTypeContext() {
      var action = new Mock<IObjectAction>();

      _userGenerator.AddAction(action.Object);
      var user = _userGenerator.Get();

      action.Verify(x => x.Enact(It.Is<IGenerationContext>(
        y => y.Node is TypeGenerationContextNode), user), Times.Once());
   }

   [Fact]
   public void SingleAppliesExtraActions() {
      var action = new Mock<IObjectAction>();
      object? actionObject = null;
      action.Setup(x => x.Enact(It.IsAny<IGenerationContext>(), It.IsAny<object>()))
        .Callback((IGenerationSession session, object dest) => { actionObject = dest; });
      _userGenerator.AddAction(action.Object);
      var user = _userGenerator.Get();

      user.Should().Be(actionObject);
   }

   [Fact]
   public void ImposeOverridesDataSource() {
      var newValue = "SomethingElse";
      var user = _userGenerator.Impose(x => x.EmailAddress, newValue).Get();

      user.FirstName.Should().Be(_testPropertyValue);
      user.LastName.Should().Be(_testPropertyValue);
      user.EmailAddress.Should().Be(newValue);
   }

   [Fact]
   public void ImposeReturnsGenerator() {
      var generator = _userGenerator.Impose(x => x.EmailAddress, "");

      generator.Should().Be(_userGenerator);
   }

   [Fact]
   public void InvokeWithFuncReturnsGenerator() {
      var generator = _methodGenerator.Invoke(
        x => x.ReturnSomething());

      generator.Should().Be(_methodGenerator);
   }

   [Fact]
   public void InvokeWithActionReturnsGenerator() {
      var generator = _methodGenerator.Invoke(
        x => x.SetSomething("Test"));

      generator.Should().Be(_methodGenerator);
   }
}