using FluentAssertions;
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
      user.EmailAddress.Should().NotBeNull();
   }

   [Fact]
   public void SimpleUserRoleIsNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.Role.Should().NotBeNull();
   }

   [Fact]
   public void SimpleUserFirstNameNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.FirstName.Should().NotBeNull();
   }

   [Fact]
   public void SimpleUserLastNameNotNull() {
      var user = _session.Single<SimpleUser>().Get();
      user.LastName.Should().NotBeNull();
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
}