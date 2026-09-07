using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingDefaultConventionsWithNoExplicitSetup {

   public WhenUsingDefaultConventionsWithNoExplicitSetup()
      => _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.Include<SimpleUser>();
         x.Include<SimpleUserRole>();
         x.Include<SimpleFieldClass>();
         x.Include<SimplePropertyClass>();
         x.Include<DefaultPropertyClass>();
         x.Include<DefaultFieldClass>();
      })
        .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void SimpleUserEmailIsNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.EmailAddress.ShouldNotBeNull();
   }

   [Fact]
   public void SimpleUserRoleIsNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.Role.ShouldNotBeNull();
   }

   [Fact]
   public void SimpleUserFirstNameNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.FirstName.ShouldNotBeNull();
   }

   [Fact]
   public void SimpleUserLastNameNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.LastName.ShouldNotBeNull();
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
}