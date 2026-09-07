using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingDefaultConventionsWithExplicitSetup {
   private readonly IGenerationSession _session;

   public WhenUsingDefaultConventionsWithExplicitSetup() => _session = AutoPocoContainer.Configure(x => {
      x.Conventions(c => { c.UseDefaultConventions(); });

      x.Include<SimpleMethodClass>()
        .Invoke(c => c.SetSomething(
          Use.Source<string, RandomStringSource>(5, 10)!,
          Use.Source<string, LastNameSource>()!))
        .Invoke(c => c.ReturnSomething());

      x.Include<SimpleUser>()
        .Setup(c => c.EmailAddress).Use<EmailAddressSource>()
        .Setup(c => c.FirstName).Use<FirstNameSource>()
        .Setup(c => c.LastName).Use<LastNameSource>();

      x.Include<SimpleUserRole>()
        .Setup(c => c.Name).Random(5, 10);

      x.Include<SimpleFieldClass>();
      x.Include<SimplePropertyClass>();
      x.Include<DefaultPropertyClass>();
      x.Include<DefaultFieldClass>();
   })
        .CreateSession();

   [Fact]
   public void SingleSimpleMethodClassReturnSomethingInvoked() {
      var result = _session.Single<SimpleMethodClass>().Get();
      Assert.True(result.ReturnSomethingCalled);
   }

   [Fact]
   public void SingleSimpleMethodClassSetSomethingSetsValueCorrectlyFromSource() {
      var result = _session.Single<SimpleMethodClass>().Get();
      result.Value!.Length.ShouldBeInRange(5, 10);
   }

   [Fact]
   public void SingleSimpleMethodClassSetSomethingSetsOtherValueCorrectlyFromSource() {
      var result = _session.Single<SimpleMethodClass>().Get();
      result.OtherValue!.Length.ShouldBeGreaterThanOrEqualTo(2);
   }

   [Fact]
   public void SingleSimpleUserRoleHasRandomName() {
      var role = _session.Single<SimpleUserRole>().Get();
      role.Name.Length.ShouldBeInRange(5, 10);
   }

   [Fact]
   public void SingleSimpleUserHasValidEmailAddress() {
      var user = _session.Single<SimpleUser>().Get();
      user.EmailAddress.Count(c => c == '@').ShouldBe(1);
   }

   [Fact]
   public void SingleSimpleSeveralUsersHaveUniqueEmailAddresses() {
      var users = _session.List<SimpleUser>(10).Get().ToArray();

      users.Where(x => users.Count(y => y.EmailAddress == x.EmailAddress) > 1).Count().ShouldBe(0);
   }

   [Fact]
   public void SingleSimpleUserImposeCustomEmailAddressHasCustomEmailAddress() {
      var user = _session.Single<SimpleUser>()
        .Impose(x => x.EmailAddress, "override@override.com")
        .Impose(x => x.FirstName, "Override")
        .Impose(x => x.LastName, "Override")
        .Get();

      user.EmailAddress.ShouldBe("override@override.com");
   }

   [Fact]
   public void SingleSimpleUserHasValidFirstName() {
      var user = _session.Single<SimpleUser>().Get();
      user.FirstName.Length.ShouldBeGreaterThan(2);
   }

   [Fact]
   public void SingleSimpleUserHasValidLastName() {
      var user = _session.Single<SimpleUser>().Get();
      user.LastName.Length.ShouldBeGreaterThan(2);
   }

   [Fact]
   public void SimpleFieldClassSomePropertyNotNull() {
      var fieldClass = _session.Single<SimpleFieldClass>().Get();
      fieldClass.SomeField.ShouldNotBeNull();
   }

   [Fact]
   public void SimpleFieldClassSomeOtherPropertyNotNull() {
      var fieldClass = _session.Single<SimpleFieldClass>().Get();
      fieldClass.SomeOtherField.ShouldNotBeNull();
   }

   [Fact]
   public void DefaultPropertyClassStringIsEmpty() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.String.ShouldBeEmpty();
   }

   [Fact]
   public void DefaultPropertyClassFloatEqualsZero() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Float.ShouldBe(0);
   }

   [Fact]
   public void DefaultPropertyClassIntegerEqualsZero() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Integer.ShouldBe(0);
   }

   [Fact]
   public void DefaultPropertyClassDateTimeIsMin() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Date.ShouldBe(DateTime.MinValue);
   }

   [Fact]
   public void DefaultFieldClassStringIsEmpty() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.String.ShouldBeEmpty();
   }

   [Fact]
   public void DefaultFieldClassFloatEqualsZero() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Float.ShouldBe(0);
   }

   [Fact]
   public void DefaultFieldClassIntegerEqualsZero() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Integer.ShouldBe(0);
   }

   [Fact]
   public void DefaultFieldClassDateTimeIsMin() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Date.ShouldBe(DateTime.MinValue);
   }

   [Fact]
   public void ListSimpleUserReturnsList() {
      var list = _session.List<SimpleUser>(10)
        .First(5)
        .Impose(x => x.LastName, "first")
        .Next(5)
        .Impose(x => x.LastName, "last")
        .All().Get();

      list.Count().ShouldBe(10);
      list.Count(x => x.LastName == "first").ShouldBe(5);
      list.Count(x => x.LastName == "last").ShouldBe(5);
   }

   [Fact]
   public void ListSimpleUserFirstHasUniqueName() {
   }

   [Fact]
   public void ListSimpleUserRandomHaveSameName() {
   }
}