using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingCustomConventionsWithNoExplicitSetup {

   public WhenUsingCustomConventionsWithNoExplicitSetup() => _session = AutoPocoContainer.Configure(x => {
      x.Conventions(c => {
         c.Register<TestTypeConvention>();
         c.Register<TestPropertyConvention>();
         c.Register<TestFieldConvention>();
      });
      x.Include<TestType>();
   })
        .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void TestPropertyHasTestValue() {
      var testType = _session.Single<TestType>().Get();
      testType.TestProperty.Should().Be("Test");
   }

   [Fact]
   public void TestFieldHasTestValue() {
      var testType = _session.Single<TestType>().Get();
      testType.TestField.Should().Be("Test");
   }

   [Fact]
   public void TestEmptyFieldIsNull() {
      var testType = _session.Single<TestType>().Get();
      testType.TestEmptyField.Should().BeNull();
   }

   [Fact]
   public void TestEmptyPropertyIsNull() {
      var testType = _session.Single<TestType>().Get();
      testType.TestEmptyProperty.Should().BeNull();
   }

   public class TestType {
      public required string TestEmptyField;

      public required string TestField;

      public required string TestProperty { get; set; }

      public required string TestEmptyProperty { get; set; }
   }

   public class TestTypeConvention : ITypeConvention {
      public void Apply(ITypeConventionContext context) {
         context.RegisterProperty(typeof(TestType).GetProperty("TestProperty")!);
         context.RegisterField(typeof(TestType).GetField("TestField")!);
      }
   }

   public class TestFieldConvention : ITypeFieldConvention {
      public void Apply(ITypeFieldConventionContext context) => context.SetValue("Test");

      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) {
      }
   }

   public class TestPropertyConvention : ITypePropertyConvention {
      public void Apply(ITypePropertyConventionContext context) => context.SetValue("Test");

      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) {
      }
   }
}