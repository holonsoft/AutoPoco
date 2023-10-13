using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationTypeBuilderTests {
   [Fact]
   public void GenericSetupCtorWithFactorySetsFactory() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleCtorClass>();
      configuration.ConstructWith<TestFactory>();

      var t = ((IEngineConfigurationTypeProvider) configuration).GetFactory();

      t!.Build()!.GetType().Should().Be(typeof(TestFactory));

   }

   [Fact]
   public void GenericSetupCtorWithFactoryWithArgsSetsFactoryWithArgs() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleCtorClass>();
      configuration.ConstructWith<TestFactory>("one", "two");

      var t = (TestFactory) ((IEngineConfigurationTypeProvider) configuration).GetFactory()!.Build()!;

      t.ArgOne.Should().Be("one");
      t.ArgTwo.Should().Be("two");
   }

   [Fact]
   public void NonGenericSetupWithPropertyReturnsMemberConfiguration() {
      IEngineConfigurationTypeBuilder configuration = new EngineConfigurationTypeBuilder(typeof(SimplePropertyClass));
      var memberConfiguration = configuration.SetupProperty("SomeProperty");

      memberConfiguration.Should().NotBeNull();
   }

   [Fact]
   public void NonGenericSetupWithFieldReturnsMemberConfiguration() {
      IEngineConfigurationTypeBuilder configuration = new EngineConfigurationTypeBuilder(typeof(SimpleFieldClass));
      var memberConfiguration = configuration.SetupField("SomeField");

      memberConfiguration.Should().NotBeNull();
   }

   [Fact]
   public void NonGenericSetupWithNonExistentPropertyThrowsArgumentException() {
      IEngineConfigurationTypeBuilder configuration = new EngineConfigurationTypeBuilder(typeof(SimplePropertyClass));

      Assert.Throws<ArgumentException>(() => { configuration.SetupProperty("SomeNonExistantProperty"); });
   }

   [Fact]
   public void NonGenericSetupWithNonExistentFieldThrowsArgumentException() {
      IEngineConfigurationTypeBuilder configuration = new EngineConfigurationTypeBuilder(typeof(SimpleFieldClass));
      Assert.Throws<ArgumentException>(() => { configuration.SetupProperty("SomeNonExistantField"); });
   }

   [Fact]
   public void NonGenericSetupMethodWithNonExistentMethodThrowsArgumentException() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleMethodClass));
      Assert.Throws<ArgumentException>(() => { configuration.SetupMethod("DoesNotExist"); });
   }

   [Fact]
   public void GenericInvokeWithFuncReturnsConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleMethodClass>();
      var returnValue = configuration.Invoke(x => x.SetSomething("Something"));

      returnValue.Should().Be(configuration);
   }

   [Fact]
   public void GenericInvokeWithActionReturnsConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleMethodClass>();
      var returnValue = configuration.Invoke(x => x.ReturnSomething());

      returnValue.Should().Be(configuration);
   }

   [Fact]
   public void NonGenericSetupMethodReturnsConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleMethodClass));
      var returnValue = configuration.SetupMethod("ReturnSomething");

      returnValue.Should().Be(configuration);
   }

   [Fact]
   public void NonGenericSetupMethodWithParametersReturnsConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleMethodClass));
      var context = new MethodInvocationContext();
      context.AddArgumentValue("Hello");
      var returnValue = configuration.SetupMethod("SetSomething", context);

      returnValue.Should().Be(configuration);
   }

   [Fact]
   public void GenericSetupWithPropertyReturnsMemberConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder<SimplePropertyClass>();
      var memberConfiguration = configuration.Setup(x => x.SomeProperty);

      Assert.NotNull(memberConfiguration);
   }

   [Fact]
   public void GenericSetupWithFieldReturnsMemberConfiguration() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleFieldClass>();
      var memberConfiguration = configuration.Setup(x => x.SomeField);

      memberConfiguration.Should().NotBeNull();
   }

   [Fact]
   public void GetConfigurationTypeReturnsType() {
      IEngineConfigurationTypeProvider configuration = new EngineConfigurationTypeBuilder<SimpleFieldClass>();
      var type = configuration.GetConfigurationType();

      type.Should().Be(typeof(SimpleFieldClass));
   }

   [Fact]
   public void GetConfigurationMembersReturnsMembers() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleFieldClass>();

      configuration.Setup(x => x.SomeField);
      ((IEngineConfigurationTypeBuilder) configuration).SetupField("SomeOtherField");

      var members = ((IEngineConfigurationTypeProvider) configuration).GetConfigurationMembers();

      members.Should().HaveCount(2);
   }

   [Fact]
   public void GetConfigurationMembersWithMethodsReturnsMembers() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleMethodClass>();

      configuration.Invoke(x => x.SetSomething("Literal"));
      configuration.SetupMethod("ReturnSomething");

      var members = ((IEngineConfigurationTypeProvider) configuration).GetConfigurationMembers();

      members.Should().HaveCount(2);
   }

   [Fact]
   public void InvokeWithInvalidMethodCallThrowsArgumentException() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleMethodClass>();
      Assert.Throws<ArgumentException>(() => {
         configuration.Invoke(x => x.SetSomething(new SimpleMethodClass().ReturnSomething()));
      });
   }

   [Fact]
   public void InvokeWithInvalidGenericMethodCallThrowsArgumentException() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleMethodClass>();
      Assert.Throws<ArgumentException>(() => {
         configuration.Invoke(x => x.SetSomething(TestGenericClass.Something<string>()!));
      });
   }

   public static class TestGenericClass {
      public static T? Something<T>() => default;
   }

   public class TestFactory : IDataSource<SimpleCtorClass> {
      public readonly string? ArgOne;
      public readonly string? ArgTwo;

      public TestFactory() {
         ArgOne = null;
         ArgTwo = null;
      }

      public TestFactory(string argOne, string argTwo) {
         ArgOne = argOne;
         ArgTwo = argTwo;
      }

      public object InternalNext(IGenerationContext? context) => throw new NotImplementedException();
   }
}