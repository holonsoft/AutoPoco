using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class ConstructorResolverTests {
   private static EngineTypeMember Property<T>(string name)
      => new EngineTypePropertyMember(typeof(T).GetProperty(name)!);

   private static EngineTypeMember Field<T>(string name)
      => new EngineTypeFieldMember(typeof(T).GetField(name)!);

   [Fact]
   public void WithoutMembersTheConstructorWithTheFewestParametersWins() {
      ConstructorResolver.Resolve(typeof(ClassWithBothCtors), []).ShouldNotBeNull().GetParameters().ShouldBeEmpty();
      ConstructorResolver.Resolve(typeof(ImmutableMoney), []).ShouldNotBeNull().GetParameters().Length.ShouldBe(1);
      ConstructorResolver.Resolve(typeof(ImmutableUserRecord), []).ShouldNotBeNull().GetParameters().Length.ShouldBe(5);
   }

   [Fact]
   public void SettableMembersDoNotPullTheResolverAwayFromTheParameterlessConstructor() {
      var ctor = ConstructorResolver.Resolve(typeof(ClassWithBothCtors), [Property<ClassWithBothCtors>("Name")]);

      ctor.ShouldNotBeNull().GetParameters().ShouldBeEmpty();
   }

   [Fact]
   public void GetOnlyMembersPullTheResolverToTheConstructorThatConsumesThem() {
      var ctor = ConstructorResolver.Resolve(typeof(ImmutableMoney), [
         Property<ImmutableMoney>("Amount"),
         Property<ImmutableMoney>("Currency")
      ]);

      ctor.ShouldNotBeNull().GetParameters().Length.ShouldBe(2);
   }

   [Fact]
   public void WithEqualRequiredMatchesTheShorterConstructorWins() {
      var ctor = ConstructorResolver.Resolve(typeof(ImmutableMoney), [Property<ImmutableMoney>("Amount")]);

      ctor.ShouldNotBeNull().GetParameters().Length.ShouldBe(1);
   }

   [Fact]
   public void TypesWithoutPublicConstructorResolveToNull() {
      ConstructorResolver.Resolve(typeof(ISimpleInterface), []).ShouldBeNull();
      ConstructorResolver.Resolve(typeof(DateOnly), [Property<DateOnly>("Year")]).ShouldNotBeNull();
      ConstructorResolver.Resolve(typeof(int), []).ShouldBeNull();
   }

   [Fact]
   public void MatchesIgnoresCaseAndRequiresAnAssignableType() {
      var parameter = typeof(ImmutableMoney).GetConstructor([typeof(decimal), typeof(string)])!.GetParameters()[1];

      ConstructorResolver.Matches(parameter, "Currency", typeof(string)).ShouldBeTrue();
      ConstructorResolver.Matches(parameter, "CURRENCY", typeof(string)).ShouldBeTrue();
      ConstructorResolver.Matches(parameter, "Currency", typeof(int)).ShouldBeFalse();
      ConstructorResolver.Matches(parameter, "Amount", typeof(string)).ShouldBeFalse();
   }

   [Fact]
   public void MatchesAcceptsDerivedTypesAndRejectsMethods() {
      var parameter = typeof(ImmutableUserRecord).GetConstructors()[0].GetParameters().Single(p => p.Name == "Role");

      ConstructorResolver.Matches(parameter, "role", typeof(ImmutableRoleRecord)).ShouldBeTrue();
      ConstructorResolver.Matches(parameter, "role", typeof(object)).ShouldBeFalse();
      ConstructorResolver.Matches(parameter, new EngineTypeMethodMember(typeof(SimpleUserRecord).GetMethod("SetPassword")!)).ShouldBeFalse();
   }

   [Fact]
   public void HasMatchingParameterLooksAtEveryPublicConstructor() {
      ConstructorResolver.HasMatchingParameter(typeof(ImmutableMoney), "Currency", typeof(string)).ShouldBeTrue();
      ConstructorResolver.HasMatchingParameter(typeof(ImmutableMoney), "Total", typeof(decimal)).ShouldBeFalse();
      ConstructorResolver.HasMatchingParameter(typeof(ClassWithUnmatchedReadOnlyProperty), "Total", typeof(decimal)).ShouldBeFalse();
      ConstructorResolver.HasMatchingParameter(typeof(SimpleUser), "FirstName", typeof(string)).ShouldBeFalse();
   }

   [Fact]
   public void CanBeSetKnowsSettersOfEveryVisibility() {
      ConstructorResolver.CanBeSet(Property<ClassWithBothCtors>("Name")).ShouldBeTrue();
      ConstructorResolver.CanBeSet(Property<ImmutableUserRecord>("FirstName")).ShouldBeTrue("init setter");
      ConstructorResolver.CanBeSet(Property<ImmutableMoney>("Amount")).ShouldBeFalse();
      ConstructorResolver.CanBeSet(Field<NumericMembersClass>("Field")).ShouldBeTrue();
   }

   [Fact]
   public void FindMemberReturnsTheFirstMatchingMember() {
      var parameter = typeof(ImmutableMoney).GetConstructor([typeof(decimal), typeof(string)])!.GetParameters()[0];
      var amount = Property<ImmutableMoney>("Amount");

      ConstructorResolver.FindMember(parameter, [Property<ImmutableMoney>("Currency"), amount]).ShouldBeSameAs(amount);
      ConstructorResolver.FindMember(parameter, [Property<ImmutableMoney>("Currency")]).ShouldBeNull();
   }
}
