using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class ObjectBuilderTests {
   private IGenerationContext CreateDummyContext() {
      var context = new Mock<IGenerationContext>();
      context.SetupGet(x => x.Node).Returns(new Mock<IGenerationContextNode>().Object);
      context.SetupGet(x => x.Builders).Returns(new Mock<IGenerationConfiguration>().Object);
      context.SetupGet(x => x.Builders.RecursionLimit).Returns(10);
      context.SetupGet(x => x.Depth).Returns(0);
      return context.Object;
   }

   [Fact]
   public void CreateObjectUsesFactoryToCreateObject() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleCtorClass));
      type.Setup(x => x.GetFactory()).Returns(new AutoPocoDataSourceFactory(typeof(TestFactory)));

      var builder = new ObjectBuilder(type.Object);
      var result = builder.CreateObject(CreateDummyContext()) as SimpleCtorClass;

      result!.ReadOnlyProperty.Should().Be("one");
   }

   [Fact]
   public void CreateObjectReturnsObject() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));

      var builder = new ObjectBuilder(type.Object);
      var user = builder.CreateObject(CreateDummyContext()) as SimpleUser;
      user.Should().NotBeNull();
   }

   [Fact]
   public void CreateObjectAppliesActionsToObject() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();

      object? obj = null;
      actionMock.Setup(x => x.Enact(It.IsAny<IGenerationContext>(), It.IsAny<object>()))
        .Callback((IGenerationSession session, object enactObject) => { obj = enactObject; });

      builder.AddAction(actionMock.Object);
      var createdObject = builder.CreateObject(CreateDummyContext());

      createdObject.Should().Be(obj);
   }

   [Fact]
   public void AddActionAddsAction() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();
      builder.AddAction(actionMock.Object);

      builder.Actions.Count(x => x == actionMock.Object).Should().Be(1);
   }

   [Fact]
   public void RemoveActionRemovesAction() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();

      builder.AddAction(actionMock.Object);
      builder.RemoveAction(actionMock.Object);

      builder.Actions.Count(x => x == actionMock.Object).Should().Be(0);
   }

   [Fact]
   public void ClearActionsRemovesAllActions() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();
      var actionMock2 = new Mock<IObjectAction>();

      builder.AddAction(actionMock.Object);
      builder.AddAction(actionMock2.Object);

      builder.ClearActions();

      builder.Actions.Count().Should().Be(0);
   }

   [Fact]
   public void CreateWrapsContextWithTypeContext() {
      var builderRepository = new Mock<IGenerationConfiguration>();
      builderRepository.SetupGet(x => x.RecursionLimit).Returns(10);
      var parent = new Mock<IGenerationContextNode>().Object;
      var context = new GenerationContext(builderRepository.Object, parent);
      var actionMock = new Mock<IObjectAction>();
      var builder = new ObjectBuilder(new EngineConfigurationType(typeof(SimpleUser)));

      builder.AddAction(actionMock.Object);
      builder.CreateObject(context);

      actionMock.Verify(
        x => x.Enact(It.Is<IGenerationContext>(y => y.Node is TypeGenerationContextNode), It.IsAny<SimpleUser>()),
        Times.Once());
   }

   public class TestFactory : IDataSource<SimpleCtorClass> {
      private readonly string _value = "one";

      public TestFactory() {
      }

      public TestFactory(string value) => _value = value;

      public object InternalNext(IGenerationContext? context) => new SimpleCtorClass(_value);
   }
}