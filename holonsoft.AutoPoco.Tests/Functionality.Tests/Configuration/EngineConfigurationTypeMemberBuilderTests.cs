using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationTypeMemberBuilderTests {
   [Fact]
   public void NotGenericUseReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      var returnedConfiguration = propertyConfiguration.Use(typeof(SimpleDataSource));

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void NotGenericUseInvalidDataSourceThrowsArgumentException() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      Assert.Throws<ArgumentException>(() => { propertyConfiguration.Use(typeof(SimpleUser)); });
   }

   [Fact]
   public void NotGenericUseWithArgsReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      var returnedConfiguration = propertyConfiguration.Use(typeof(SimpleDataSource), 0, 1, 10);

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void NotGenericUseInvalidDataSourceWithArgsThrowsArgumentException() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      Assert.Throws<ArgumentException>(() => { propertyConfiguration.Use(typeof(SimpleUser), 0, 1, 10); });
   }

   [Fact]
   public void NotGenericDefaultReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      var returnedConfiguration = propertyConfiguration.Default();

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void NotGenericDefaultResetsSource() {
      var configuration = new EngineConfigurationTypeBuilder(typeof(SimpleUser));
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder(null, configuration);

      propertyConfiguration.Use(typeof(SimpleDataSource));
      propertyConfiguration.Default();

      propertyConfiguration.GetDataSources().Should().HaveCount(0);
   }

   [Fact]
   public void GenericUseReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(null!, configuration);

      var returnedConfiguration = propertyConfiguration.Use<SimpleDataSource>();

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void GenericUseWithArgsReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(null!, configuration);

      var returnedConfiguration = propertyConfiguration.Use<SimpleDataSource>(0, 1, 10);

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void GenericDefaultReturnsTypeBuilder() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(null!, configuration);

      var returnedConfiguration = propertyConfiguration.Default();

      returnedConfiguration.Should().Be(configuration);
   }

   [Fact]
   public void GenericDefaultResetsSource() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(null!, configuration);

      propertyConfiguration.Use<SimpleDataSource>();
      propertyConfiguration.Default();

      propertyConfiguration.GetDataSources().Should().HaveCount(0);
   }

   [Fact]
   public void GenericGetConfigurationMemberReturnsConfigurationMember() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var member = ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);

      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(member, configuration);

      var returnMember = propertyConfiguration.GetConfigurationMember();
      returnMember.Should().Be(member);
   }

   [Fact]
   public void GenericGetConfigurationActionInvalidReturnsNULL() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var member = ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);

      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(member, configuration);

      var returnAction = propertyConfiguration.GetDataSources().FirstOrDefault();
      returnAction.Should().BeNull();
   }

   [Fact]
   public void GenericGetConfigurationActionValidReturnsConfigurationAction() {
      var configuration = new EngineConfigurationTypeBuilder<SimpleUser>();
      var member = ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);

      var propertyConfiguration = new EngineConfigurationTypeMemberBuilder<SimpleUser, string>(member, configuration);
      propertyConfiguration.Use<SimpleDataSource>();

      var returnAction = propertyConfiguration.GetDataSources().FirstOrDefault();
      returnAction.Should().NotBeNull();
   }
}