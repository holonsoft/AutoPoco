using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources;
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
      result.Value!.Length.Should().BeInRange(5, 10);
   }

   [Fact]
   public void SingleSimpleMethodClassSetSomethingSetsOtherValueCorrectlyFromSource() {
      var result = _session.Single<SimpleMethodClass>().Get();
      result.OtherValue!.Length.Should().BeGreaterThanOrEqualTo(2);
   }

   [Fact]
   public void SingleSimpleUserRoleHasRandomName() {
      var role = _session.Single<SimpleUserRole>().Get();
      role.Name.Length.Should().BeInRange(5, 10);
   }

   [Fact]
   public void SingleSimpleUserHasValidEmailAddress() {
      var user = _session.Single<SimpleUser>().Get();
      user.EmailAddress.Should().Contain("@", Exactly.Once());
   }

   [Fact]
   public void SingleSimpleSeveralUsersHaveUniqueEmailAddresses() {
      var users = _session.List<SimpleUser>(10).Get().ToArray();

      users.Where(x => users.Count(y => y.EmailAddress == x.EmailAddress) > 1).Count().Should().Be(0);
   }

   [Fact]
   public void SingleSimpleUserImposeCustomEmailAddressHasCustomEmailAddress() {
      var user = _session.Single<SimpleUser>()
        .Impose(x => x.EmailAddress, "override@override.com")
        .Impose(x => x.FirstName, "Override")
        .Impose(x => x.LastName, "Override")
        .Get();

      user.EmailAddress.Should().Be("override@override.com");
   }

   [Fact]
   public void SingleSimpleUserHasValidFirstName() {
      var user = _session.Single<SimpleUser>().Get();
      user.FirstName.Length.Should().BeGreaterThan(2);
   }

   [Fact]
   public void SingleSimpleUserHasValidLastName() {
      var user = _session.Single<SimpleUser>().Get();
      user.LastName.Length.Should().BeGreaterThan(2);
   }

   [Fact]
   public void SimpleFieldClassSomePropertyNotNull() {
      var fieldClass = _session.Single<SimpleFieldClass>().Get();
      fieldClass.SomeField.Should().NotBeNull();
   }

   [Fact]
   public void SimpleFieldClassSomeOtherPropertyNotNull() {
      var fieldClass = _session.Single<SimpleFieldClass>().Get();
      fieldClass.SomeOtherField.Should().NotBeNull();
   }

   [Fact]
   public void DefaultPropertyClassStringIsEmpty() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.String.Should().BeEmpty();
   }

   [Fact]
   public void DefaultPropertyClassFloatEqualsZero() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Float.Should().Be(0);
   }

   [Fact]
   public void DefaultPropertyClassIntegerEqualsZero() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Integer.Should().Be(0);
   }

   [Fact]
   public void DefaultPropertyClassDateTimeIsMin() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.Date.Should().Be(DateTime.MinValue);
   }

   [Fact]
   public void DefaultFieldClassStringIsEmpty() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.String.Should().BeEmpty();
   }

   [Fact]
   public void DefaultFieldClassFloatEqualsZero() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Float.Should().Be(0);
   }

   [Fact]
   public void DefaultFieldClassIntegerEqualsZero() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Integer.Should().Be(0);
   }

   [Fact]
   public void DefaultFieldClassDateTimeIsMin() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.Date.Should().Be(DateTime.MinValue);
   }

   [Fact]
   public void ListSimpleUserReturnsList() {
      var list = _session.List<SimpleUser>(10)
        .First(5)
        .Impose(x => x.LastName, "first")
        .Next(5)
        .Impose(x => x.LastName, "last")
        .All().Get();

      list.Should().HaveCount(10);
      list.Count(x => x.LastName == "first").Should().Be(5);
      list.Count(x => x.LastName == "last").Should().Be(5);
   }

   [Fact]
   public void ListSimpleUserFirstHasUniqueName() {
   }

   [Fact]
   public void ListSimpleUserRandomHaveSameName() {
   }
}