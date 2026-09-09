using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenConstructingImmutableTypes {
   private static readonly DateOnly _minBirthday = new(1950, 1, 1);
   private static readonly DateOnly _maxBirthday = new(2000, 12, 31);

   private static IGenerationSession RecordSessionWithExplicitSetup(bool respectNullableAnnotations = false)
      => AutoPocoContainer.Configure(x => {
         if (respectNullableAnnotations)
            x.RespectNullableAnnotations(100);

         x.Include<ImmutableUserRecord>()
            .Setup(c => c.FirstName).Use<FirstNameSource>()
            .Setup(c => c.LastName).Use<LastNameSource>()
            .Setup(c => c.Nickname).Use<FirstNameSource>()
            .Setup(c => c.Birthday).Use<DateOnlySource>(_minBirthday, _maxBirthday);
         x.Include<ImmutableRoleRecord>()
            .Setup(c => c.Name).From(() => "Admin");
      }).CreateSession();

   [Fact]
   public void ARecordWithoutParameterlessConstructorGetsItsConfiguredValuesThroughTheConstructor() {
      var users = RecordSessionWithExplicitSetup().Collection<ImmutableUserRecord>(50).ToList();

      users.ShouldAllBe(u => !string.IsNullOrEmpty(u.FirstName));
      users.ShouldAllBe(u => !string.IsNullOrEmpty(u.LastName));
      users.ShouldAllBe(u => u.Birthday >= _minBirthday && u.Birthday <= _maxBirthday);
      users.ShouldAllBe(u => u.Role != null && u.Role.Name == "Admin");
      users.Select(u => u.FirstName).Distinct().Count().ShouldBeGreaterThan(1);
   }

   [Fact]
   public void ARecordWithDefaultConventionsAndNoSetupIsCreatedWithoutAnException() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<ImmutableUserRecord>();
      }).CreateSession();

      var user = session.Next<ImmutableUserRecord>();

      user.FirstName.ShouldBe("");
      user.LastName.ShouldBe("");
      user.Birthday.ShouldBe(default);
      user.Role.ShouldNotBeNull().Name.ShouldBe("");
   }

   [Fact]
   public void ARecordWithoutAnyConventionAndSetupIsCreatedWithDefaults() {
      var session = AutoPocoContainer.Configure(x => x.Include<ImmutableUserRecord>()).CreateSession();

      var user = session.Next<ImmutableUserRecord>();

      user.FirstName.ShouldBe("");
      user.Birthday.ShouldBe(default);
      user.Role.ShouldNotBeNull().Name.ShouldBe("");
   }

   [Fact]
   public void AnUnknownRecordIsCreatedOnRequest() {
      var session = AutoPocoContainer.Configure(x => x.Conventions(c => c.UseDefaultConventions())).CreateSession();

      session.Next<ImmutableRoleRecord>().Name.ShouldBe("");
   }

   [Fact]
   public void AnImmutableClassGetsItsConfiguredValuesThroughTheConstructor() {
      var session = AutoPocoContainer.Configure(x => x.Include<ImmutableMoney>()
            .Setup(c => c.Amount).Use<NumberSource<decimal>>(1m, 10m)
            .Setup(c => c.Currency).From(() => "CHF"))
         .CreateSession();

      var money = session.Collection<ImmutableMoney>(20).ToList();

      money.ShouldAllBe(m => m.Amount >= 1m && m.Amount <= 10m);
      money.ShouldAllBe(m => m.Currency == "CHF");
   }

   [Fact]
   public void AnImmutableClassWithPartialSetupUsesTheShortestConstructorThatTakesTheConfiguredMembers() {
      var session = AutoPocoContainer.Configure(x => x.Include<ImmutableMoney>()
            .Setup(c => c.Amount).From(() => 42m))
         .CreateSession();

      var money = session.Next<ImmutableMoney>();

      money.Amount.ShouldBe(42m);
      money.Currency.ShouldBe("EUR");
   }

   [Fact]
   public void AnImmutableClassWithDefaultConventionsIsPopulatedByTheMemberConventions() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<ImmutableMoney>();
      }).CreateSession();

      var money = session.Next<ImmutableMoney>();

      money.Currency.ShouldBe("");
   }

   [Fact]
   public void AnOrdinaryClassKeepsUsingItsParameterlessConstructor() {
      var session = AutoPocoContainer.Configure(x => x.Include<ClassWithBothCtors>()
            .Setup(c => c.Name).From(() => "configured"))
         .CreateSession();

      session.Next<ClassWithBothCtors>().Name.ShouldBe("configured");
   }

   [Fact]
   public void AGetOnlyPropertyWithoutMatchingConstructorParameterThrowsAClearMessage() {
      var session = AutoPocoContainer.Configure(x => x.Include<ClassWithUnmatchedReadOnlyProperty>()
            .Setup(c => c.Total).From(() => 1m))
         .CreateSession();

      var exception = Should.Throw<InvalidOperationException>(() => session.Next<ClassWithUnmatchedReadOnlyProperty>());

      exception.Message.ShouldContain("Total");
      exception.Message.ShouldContain("constructor");
   }

   [Fact]
   public void DeclaredParameterDefaultsAreUsedForUnconfiguredParameters() {
      var session = AutoPocoContainer.Configure(x => x.Include<ClassWithDefaultedCtorParameter>()
            .Setup(c => c.Name).From(() => "n"))
         .CreateSession();

      var result = session.Next<ClassWithDefaultedCtorParameter>();

      result.Name.ShouldBe("n");
      result.Retries.ShouldBe(3);
      result.Role.ShouldBeNull();
   }

   [Fact]
   public void ImposeAndGenerationTimeSourceStillWorkOnPositionalRecordProperties() {
      var session = RecordSessionWithExplicitSetup();

      var imposed = session.Single<ImmutableUserRecord>().Impose(u => u.FirstName, "Rob").Get();
      var sourced = session.Single<ImmutableUserRecord>().Source(u => u.LastName, () => "Ashton").Get();

      imposed.FirstName.ShouldBe("Rob");
      sourced.LastName.ShouldBe("Ashton");
   }

   [Fact]
   public void NullableAnnotationsApplyToConstructorParameters() {
      var users = RecordSessionWithExplicitSetup(respectNullableAnnotations: true).Collection<ImmutableUserRecord>(30).ToList();

      users.ShouldAllBe(u => u.Nickname == null);
      users.ShouldAllBe(u => u.FirstName != null);
   }

   [Fact]
   public void ASelfReferencingRecordHonoursTheRecursionLimit() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<TreeNodeRecord>();
      }).CreateSession(3);

      var node = session.Next<TreeNodeRecord>();
      var depth = 0;
      for (var current = node; current != null; current = current.Child)
         depth++;

      depth.ShouldBeInRange(2, 4);
   }

   [Fact]
   public void TheRecordDemoNoLongerNeedsAParameterlessConstructor() {
      var session = AutoPocoContainer.Configure(x => x.Include<ImmutableUserRecord>()
            .Setup(c => c.FirstName).Use<FirstNameSource>()
            .Setup(c => c.LastName).Use<LastNameSource>())
         .CreateSession();

      var users = session.List<ImmutableUserRecord>(10)
         .First(5).Impose(u => u.FirstName, "Rob")
         .Next(5).Impose(u => u.LastName, "Smith")
         .All()
         .Get()
         .ToList();

      users.Take(5).ShouldAllBe(u => u.FirstName == "Rob");
      users.Skip(5).ShouldAllBe(u => u.LastName == "Smith");
   }
}
