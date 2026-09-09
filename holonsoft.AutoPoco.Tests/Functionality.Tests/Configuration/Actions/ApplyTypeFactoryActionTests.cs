using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Configuration.TypeRegistrationActions;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration.Actions;

public class ApplyTypeFactoryActionTests {
   [Fact]
   public void WhenActionIsAppliedFactoryIsAppliedFromConfiguration() {
      var provider = new Mock<IEngineConfigurationProvider>();
      var targetType = new Mock<IEngineConfigurationType>();
      var targetTypeProvider = new Mock<IEngineConfigurationTypeProvider>();
      var factory = new Mock<IEngineConfigurationDataSource>();

      targetType.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      targetTypeProvider.Setup(x => x.GetConfigurationType()).Returns(typeof(SimpleUser));

      provider.Setup(x => x.GetConfigurationTypes()).Returns(new[] { targetTypeProvider.Object });
      targetTypeProvider.Setup(x => x.GetFactory()).Returns(factory.Object);

      var action = new ApplyTypeFactoryAction(provider.Object);
      action.Apply(targetType.Object);

      targetType.Verify(x => x.SetFactory(factory.Object), Times.Once());
   }

   [Fact]
   public void WhenActionIsAppliedFactoryWithNoConfigurationOrConventionAvailableClassesGetACtorSource() {
      var provider = new Mock<IEngineConfigurationProvider>();
      var targetType = new Mock<IEngineConfigurationType>();
      targetType.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      provider.Setup(x => x.GetConfigurationTypes()).Returns(Array.Empty<IEngineConfigurationTypeProvider>());

      var action = new ApplyTypeFactoryAction(provider.Object);
      action.Apply(targetType.Object);

      targetType.Verify(x => x.SetFactory(
          It.Is<IEngineConfigurationDataSource>(y => y.Build() is CtorSource<SimpleUser>)),
        Times.Once());
   }

   [Theory]
   [InlineData(typeof(ISimpleInterface))]
   [InlineData(typeof(Stream))]
   [InlineData(typeof(DateOnly))]
   [InlineData(typeof(string))]
   public void WhenActionIsAppliedFactoryWithNoConfigurationOrConventionAvailableOtherTypesGetTheDefaultSource(Type type) {
      var provider = new Mock<IEngineConfigurationProvider>();
      var targetType = new Mock<IEngineConfigurationType>();
      targetType.SetupGet(x => x.RegisteredType).Returns(type);
      provider.Setup(x => x.GetConfigurationTypes()).Returns(Array.Empty<IEngineConfigurationTypeProvider>());

      var action = new ApplyTypeFactoryAction(provider.Object);
      action.Apply(targetType.Object);

      targetType.Verify(x => x.SetFactory(
          It.Is<IEngineConfigurationDataSource>(y => y.Build()!.GetType().GetGenericTypeDefinition() == typeof(DefaultSource<>))),
        Times.Once());
   }

   [Fact]
   public void WhenActionIsAppliedAnExistingConventionFactoryIsKept() {
      var provider = new Mock<IEngineConfigurationProvider>();
      var targetType = new Mock<IEngineConfigurationType>();
      var factory = new Mock<IEngineConfigurationDataSource>();
      targetType.SetupGet(x => x.RegisteredType).Returns(typeof(SimpleUser));
      targetType.Setup(x => x.GetFactory()).Returns(factory.Object);
      provider.Setup(x => x.GetConfigurationTypes()).Returns(Array.Empty<IEngineConfigurationTypeProvider>());

      var action = new ApplyTypeFactoryAction(provider.Object);
      action.Apply(targetType.Object);

      targetType.Verify(x => x.SetFactory(It.IsAny<IEngineConfigurationDataSource>()), Times.Never());
   }
}