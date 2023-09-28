using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingNoConventionsWithNoExplicitSetup {

   public WhenUsingNoConventionsWithNoExplicitSetup() => _session = AutoPocoContainer.Configure(x => {
      x.Include<DefaultPropertyClass>();
      x.Include<DefaultFieldClass>();
   })
        .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void DefaultPropertyClassStringIsNull() {
      var propertyClass = _session.Single<DefaultPropertyClass>().Get();
      propertyClass.String.Should().BeNull();
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
   public void DefaultFieldClassStringIsNull() {
      var propertyClass = _session.Single<DefaultFieldClass>().Get();
      propertyClass.String.Should().Be(null);
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