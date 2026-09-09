using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
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

      result!.ReadOnlyProperty.ShouldBe("one");
   }

   [Fact]
   public void CreateObjectReturnsObject() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));

      var builder = new ObjectBuilder(type.Object);
      var user = builder.CreateObject(CreateDummyContext()) as SimpleUser;
      user.ShouldNotBeNull();
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

      createdObject.ShouldBe(obj);
   }

   [Fact]
   public void AddActionAddsAction() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();
      builder.AddAction(actionMock.Object);

      builder.Actions.Count(x => x == actionMock.Object).ShouldBe(1);
   }

   [Fact]
   public void RemoveActionRemovesAction() {
      var type = new Mock<IEngineConfigurationType>();
      type.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      var builder = new ObjectBuilder(type.Object);
      var actionMock = new Mock<IObjectAction>();

      builder.AddAction(actionMock.Object);
      builder.RemoveAction(actionMock.Object);

      builder.Actions.Count(x => x == actionMock.Object).ShouldBe(0);
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

      builder.Actions.Count().ShouldBe(0);
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

   private static EngineConfigurationType TypeWithSources(Type registeredType, params string[] propertyNames) {
      var type = new EngineConfigurationType(registeredType);
      foreach (var name in propertyNames) {
         var member = new EngineTypePropertyMember(registeredType.GetProperty(name)!);
         type.RegisterMember(member);
         type.GetRegisteredMember(member).SetDataSource(FuncFactory(() => name.ToLowerInvariant()));
      }

      return type;
   }

   private static AutoPocoDataSourceFactory FuncFactory<T>(Func<T> func) {
      var factory = new AutoPocoDataSourceFactory(typeof(FuncSource<T>));
      factory.SetParams(func);
      return factory;
   }

   [Fact]
   public void MembersConsumedByAMemberBoundFactoryGetNoAction() {
      var type = TypeWithSources(typeof(ImmutableMoney), nameof(ImmutableMoney.Currency));
      type.SetFactory(new AutoPocoDataSourceFactory(typeof(CtorSource<ImmutableMoney>)));

      var builder = new ObjectBuilder(type);
      var result = (ImmutableMoney) builder.CreateObject(CreateDummyContext());

      builder.Actions.ShouldBeEmpty();
      result.Currency.ShouldBe("currency");
   }

   [Fact]
   public void SettableMembersNotConsumedByTheFactoryKeepTheirAction() {
      var type = TypeWithSources(typeof(SimpleUser), nameof(SimpleUser.FirstName));
      type.SetFactory(new AutoPocoDataSourceFactory(typeof(CtorSource<SimpleUser>)));

      var builder = new ObjectBuilder(type);
      var result = (SimpleUser) builder.CreateObject(CreateDummyContext());

      builder.Actions.Count().ShouldBe(1);
      result.FirstName.ShouldBe("firstname");
   }

   [Fact]
   public void GetOnlyMembersWithoutAConstructorParameterThrowAClearMessage() {
      var type = TypeWithSources(typeof(ClassWithUnmatchedReadOnlyProperty), nameof(ClassWithUnmatchedReadOnlyProperty.Total));
      type.GetRegisteredMember(new EngineTypePropertyMember(typeof(ClassWithUnmatchedReadOnlyProperty).GetProperty("Total")!))
         .SetDataSource(FuncFactory(() => 1m));
      type.SetFactory(new AutoPocoDataSourceFactory(typeof(CtorSource<ClassWithUnmatchedReadOnlyProperty>)));

      var exception = Should.Throw<InvalidOperationException>(() => new ObjectBuilder(type));

      exception.Message.ShouldContain("Total");
      exception.Message.ShouldContain(nameof(ClassWithUnmatchedReadOnlyProperty));
   }

   [Fact]
   public void GetOnlyMembersAreSkippedWhenACustomFactoryIsInCharge() {
      var type = TypeWithSources(typeof(SimpleCtorClass), nameof(SimpleCtorClass.ReadOnlyProperty));
      type.SetFactory(new AutoPocoDataSourceFactory(typeof(TestFactory)));

      var builder = new ObjectBuilder(type);
      var result = (SimpleCtorClass) builder.CreateObject(CreateDummyContext());

      builder.Actions.ShouldBeEmpty();
      result.ReadOnlyProperty.ShouldBe("one");
   }

   public class TestFactory : IDataSource<SimpleCtorClass> {
      private readonly string _value = "one";

      public TestFactory() {
      }

      public TestFactory(string value) => _value = value;

      public object InternalNext(IGenerationContext? context) => new SimpleCtorClass(_value);
   }
}