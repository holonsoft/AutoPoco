using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class CtorSourceTests {
   private static MemberSource Bind<T>(string propertyName, object? value)
      => new(new EngineTypePropertyMember(typeof(T).GetProperty(propertyName)!), new FuncSource<object?>(() => value));

   private static IGenerationContext ContextWithLimit(int depth, int limit) {
      var builders = new Mock<IGenerationConfiguration>();
      builders.SetupGet(x => x.RecursionLimit).Returns(limit);
      var context = new Mock<IGenerationContext>();
      context.SetupGet(x => x.Builders).Returns(builders.Object);
      context.SetupGet(x => x.Depth).Returns(depth);
      return context.Object;
   }

   [Fact]
   public void CtorSourceCallsCtorRequestsArgumentsFromSession() {
      var source = new CtorSource<TestCtorObject>(
        typeof(TestCtorObject).GetConstructor(new[] { typeof(TestDependency) })!
      );

      var context = new Mock<IGenerationContext>();
      context.Setup(x => x.Next<TestDependency>()).Returns(new TestDependency());

      var result = source.Next(context.Object);

      result.Dependency.ShouldNotBeNull();
   }

   [Fact]
   public void WithoutBoundMembersTheConstructorWithTheFewestParametersIsUsed() {
      var source = new CtorSource<ImmutableMoney>();

      var result = source.Next(null);

      source.Constructor.ShouldNotBeNull().GetParameters().Length.ShouldBe(1);
      result.Amount.ShouldBe(0m);
      result.Currency.ShouldBe("EUR");
   }

   [Fact]
   public void BoundMembersFeedTheConstructorAndAreReportedAsConsumed() {
      var source = new CtorSource<ImmutableMoney>();
      var amount = Bind<ImmutableMoney>("Amount", 12.5m);
      var currency = Bind<ImmutableMoney>("Currency", "CHF");

      var consumed = source.BindMembers([amount, currency]);
      var result = source.Next(ContextWithLimit(0, 5));

      consumed.ShouldBe([amount.Member, currency.Member]);
      source.Constructor.ShouldNotBeNull().GetParameters().Length.ShouldBe(2);
      result.Amount.ShouldBe(12.5m);
      result.Currency.ShouldBe("CHF");
   }

   [Fact]
   public void APinnedConstructorIsKeptAndStillTakesBoundMembers() {
      var source = new CtorSource<ImmutableMoney>(typeof(ImmutableMoney).GetConstructor([typeof(decimal)])!);
      var amount = Bind<ImmutableMoney>("Amount", 3m);
      var currency = Bind<ImmutableMoney>("Currency", "CHF");

      var consumed = source.BindMembers([amount, currency]);
      var result = source.Next(ContextWithLimit(0, 5));

      consumed.ShouldBe([amount.Member]);
      result.Amount.ShouldBe(3m);
      result.Currency.ShouldBe("EUR");
   }

   [Fact]
   public void UnboundParametersUseTheirDeclaredDefaultOrTheTypeDefault() {
      var source = new CtorSource<ClassWithDefaultedCtorParameter>();
      source.BindMembers([Bind<ClassWithDefaultedCtorParameter>("Name", "n")]);

      var result = source.Next(ContextWithLimit(0, 5));

      result.Name.ShouldBe("n");
      result.Retries.ShouldBe(3);
      result.Role.ShouldBeNull();
   }

   [Fact]
   public void UnboundValueTypeParametersStayDefaultWhileReferenceTypesAreAskedFromTheSession() {
      // strict: asking for Next<DateOnly>() would fail the test
      var context = new Mock<IGenerationContext>(MockBehavior.Strict);
      context.SetupGet(x => x.Builders).Returns((IGenerationConfiguration) null!);
      context.SetupGet(x => x.Node).Returns((IGenerationContextNode) null!);
      context.Setup(x => x.Next<string>()).Returns("from session");
      context.Setup(x => x.Next<ImmutableRoleRecord>()).Returns(new ImmutableRoleRecord("role"));
      var source = new CtorSource<ImmutableUserRecord>();

      var result = source.Next(context.Object);

      result.Birthday.ShouldBe(default);
      result.FirstName.ShouldBe("from session");
      result.Role.Name.ShouldBe("role");
   }

   [Fact]
   public void StructsWithoutBoundMembersStayDefaultEvenWhenTheyHaveConstructors() {
      new CtorSource<Guid>().Next(null).ShouldBe(Guid.Empty);
      new CtorSource<DateOnly>().Next(null).ShouldBe(default);
      new CtorSource<DateOnly>(typeof(DateOnly).GetConstructor([typeof(int), typeof(int), typeof(int)])!).Next(null).ShouldBe(default);
   }

   [Fact]
   public void StructsWithBoundMembersAreConstructed() {
      var source = new CtorSource<DateOnly>();
      source.BindMembers([Bind<DateOnly>("Year", 2024), Bind<DateOnly>("Month", 2), Bind<DateOnly>("Day", 29)]);

      source.Next(ContextWithLimit(0, 5)).ShouldBe(new DateOnly(2024, 2, 29));
   }

   [Fact]
   public void BeyondTheRecursionLimitBoundSourcesAreNotAskedAndReferencesStayNull() {
      var source = new CtorSource<TreeNodeRecord>();
      var calls = 0;
      var child = new MemberSource(
         new EngineTypePropertyMember(typeof(TreeNodeRecord).GetProperty("Child")!),
         new FuncSource<TreeNodeRecord?>(() => { calls++; return new TreeNodeRecord("child", null); }));
      source.BindMembers([Bind<TreeNodeRecord>("Name", "root"), child]);

      var within = source.Next(ContextWithLimit(2, 3));
      var beyond = source.Next(ContextWithLimit(3, 3));

      within.Child.ShouldNotBeNull();
      beyond.Child.ShouldBeNull();
      beyond.Name.ShouldBeNull();
      calls.ShouldBe(1);
   }

   [Fact]
   public void ValueTypesWithoutPublicConstructorReturnDefault() {
      new CtorSource<int>().Next(null).ShouldBe(0);
      new CtorSource<decimal>().Next(null).ShouldBe(0m);
   }

   [Fact]
   public void ClassesWithoutPublicConstructorThrowAClearMessage() {
      var source = new CtorSource<ISimpleInterface>();

      Should.Throw<InvalidOperationException>(() => source.Next(null))
         .Message.ShouldContain(nameof(ISimpleInterface));
   }

   [Fact]
   public void ParentSourceFindsTheEnclosingObjectThroughTheConstructor() {
      var parent = new SimpleUserRole { Name = "parent" };
      var source = new CtorSource<TestCtorObjectWithParent>();
      source.BindMembers([new MemberSource(
         new EngineTypePropertyMember(typeof(TestCtorObjectWithParent).GetProperty("Parent")!),
         new ParentSource<SimpleUserRole>())]);

      var builders = new Mock<IGenerationConfiguration>();
      builders.SetupGet(x => x.RecursionLimit).Returns(5);
      var context = new GenerationContext(builders.Object, new TypeGenerationContextNode(null, parent));

      source.Next(context).Parent.ShouldBeSameAs(parent);
   }
}

public class TestCtorObject(TestDependency dependency) {
   public TestDependency Dependency { get; } = dependency;
}

public class TestCtorObjectWithParent(SimpleUserRole parent) {
   public SimpleUserRole Parent { get; } = parent;
}
