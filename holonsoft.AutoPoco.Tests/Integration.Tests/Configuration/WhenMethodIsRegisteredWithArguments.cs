using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Configuration;

public class WhenMethodIsRegisteredWithArguments : ConfigurationBaseTest {
   private readonly IEngineConfigurationType _engineConfigurationType;
   private readonly EngineTypeMethodMember _singleArgMethod;
   private readonly EngineTypeMethodMember _doubleArgMethod;
   protected IEngineConfiguration Configuration { get; private set; }

   public WhenMethodIsRegisteredWithArguments() {
      Builder.Include<SimpleMethodClass>()
        .Invoke(x => x.SetSomething("Literal"))
        .Invoke(x => x.SetSomething(
          Use.Source<string, FirstNameSource>()!,
          Use.Source<string, LastNameSource>()!));

      Configuration = new EngineConfigurationFactory().Create(Builder, Builder.ConventionProvider);

      // Get some info for the tests
      _engineConfigurationType = Configuration.GetRegisteredType(typeof(SimpleMethodClass))!;
      _singleArgMethod = (EngineTypeMethodMember) ReflectionHelper.GetMember(
        typeof(SimpleMethodClass).GetMethod("SetSomething", new[] { typeof(string) })!);
      _doubleArgMethod = (EngineTypeMethodMember) ReflectionHelper.GetMember(
        typeof(SimpleMethodClass).GetMethod("SetSomething", new[] { typeof(string), typeof(string) })!);
   }

   [Fact]
   public void BothMethodsAreRegistered()
      => _engineConfigurationType.GetRegisteredMembers().Count().Should().Be(2);

   [Fact]
   public void MethodWithLiteralArgumentExposedInConfiguration() {
      var member = _engineConfigurationType.GetRegisteredMember(_singleArgMethod);
      member.Should().NotBeNull();
   }

   [Fact]
   public void MethodWithLiteralArgumentHasOneDatasource() {
      var member = _engineConfigurationType.GetRegisteredMember(_singleArgMethod);
      member.GetDataSources().Count().Should().Be(1);
   }

   [Fact]
   public void MethodWithLiteralArgumentHasValueDatasource() {
      var member = _engineConfigurationType.GetRegisteredMember(_singleArgMethod);
      var configurationSource = member.GetDataSources().Single();
      var source = configurationSource.Build()!;

      source.GetType().Should().Be(typeof(ValueSource<object>));
   }

   [Fact]
   public void MethodWithDatasourceArgumentExposedInConfiguration() {
      var member = _engineConfigurationType.GetRegisteredMember(_doubleArgMethod);
      member.Should().NotBeNull();
   }

   [Fact]
   public void MethodWithTwoArgumentsHasOneTwosources() {
      var member = _engineConfigurationType.GetRegisteredMember(_doubleArgMethod);

      member.GetDataSources().Count().Should().Be(2);
   }

   [Theory]
   [InlineData(0, typeof(FirstNameSource))]
   [InlineData(1, typeof(LastNameSource))]
   public void MethodWithTwoArgumentsHasCorrectDatasource(int skip, Type expectedType) {
      var member = _engineConfigurationType.GetRegisteredMember(_doubleArgMethod);
      var sourceConfig = member.GetDataSources().Skip(skip).First();
      var source = sourceConfig.Build()!;

      source.GetType().Should().Be(expectedType);
   }
}