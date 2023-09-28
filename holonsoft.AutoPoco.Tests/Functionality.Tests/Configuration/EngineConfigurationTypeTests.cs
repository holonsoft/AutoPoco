using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationTypeTests {
   [Fact]
   public void RegisterMemberMemberAlreadyRegisteredThrowsException() {
      var type = new EngineConfigurationType(typeof(SimpleUser));
      type.RegisterMember(ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress));

      Assert.Throws<ArgumentException>(() => {
         type.RegisterMember(ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress));
      });
   }

   [Fact]
   public void RegisterMemberGetRegisteredMembersReturnsMembers() {
      var type = new EngineConfigurationType(typeof(SimpleUser));
      type.RegisterMember(ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress));
      type.RegisterMember(ReflectionHelper.GetMember<SimpleUser>(x => x.FirstName));

      var members = type.GetRegisteredMembers();
      members.Count().Should().Be(2);
   }

   [Fact]
   public void SetFactoryWithTypeSetsFactory() {
      var type = new EngineConfigurationType(typeof(SimpleUser));
      var source = new Mock<IEngineConfigurationDataSource>();
      type.SetFactory(source.Object);
      var factory = type.GetFactory();
      factory.Should().BeSameAs(source.Object);
      //Assert.AreEqual(source.Object, factory);
   }

   public class TestFactory : IDataSource<SimpleCtorClass> {
      public object Next(IGenerationContext? context) => throw new NotImplementedException();
   }
}