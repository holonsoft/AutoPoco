﻿using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Engine;

public class WhenObjectsAddedToConfiguration : GenerationSessionFactoryTestBase {
   protected override void PopulateConfiguration() {
      Configuration.RegisterType(typeof(SimpleUser));
      var simpleUserConfig = Configuration.GetRegisteredType(typeof(SimpleUser))!;

      var emailMember = ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);
      var firstNameMember = ReflectionHelper.GetMember<SimpleUser>(x => x.FirstName);
      var lastNameMember = ReflectionHelper.GetMember<SimpleUser>(x => x.LastName);

      simpleUserConfig.RegisterMember(emailMember);
      simpleUserConfig.RegisterMember(firstNameMember);
      simpleUserConfig.RegisterMember(lastNameMember);

      var emailSourceFactory = new DataSourceFactory(typeof(ValueSource<string>));
      emailSourceFactory.SetParams("test@test.com");
      var firstNameFactory = new DataSourceFactory(typeof(ValueSource<string>));
      firstNameFactory.SetParams("first");
      var lastNameFactory = new DataSourceFactory(typeof(ValueSource<string>));
      lastNameFactory.SetParams("last");

      simpleUserConfig.GetRegisteredMember(emailMember).SetDataSource(emailSourceFactory);
      simpleUserConfig.GetRegisteredMember(firstNameMember).SetDataSource(firstNameFactory);
      simpleUserConfig.GetRegisteredMember(lastNameMember).SetDataSource(lastNameFactory);

      Configuration.RegisterType(typeof(SimpleFieldClass));
      var simpleFieldConfig = Configuration.GetRegisteredType(typeof(SimpleFieldClass))!;

      var someFieldMember = ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!);
      var someOtherField = ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeOtherField!);

      simpleFieldConfig.RegisterMember(someFieldMember);
      simpleFieldConfig.RegisterMember(someOtherField);

      var someFieldFactory = new DataSourceFactory(typeof(ValueSource<string>));
      someFieldFactory.SetParams("one");
      var someOtherFieldFactory = new DataSourceFactory(typeof(ValueSource<string>));
      someOtherFieldFactory.SetParams("other");

      simpleFieldConfig.GetRegisteredMember(someFieldMember).SetDataSource(someFieldFactory);
      simpleFieldConfig.GetRegisteredMember(someOtherField).SetDataSource(someOtherFieldFactory);
   }

   [Fact]
   public void CreateSimpleFieldClassSomeFieldIsSet() {
      var simpleFieldClass = GenerationSession.Single<SimpleFieldClass>().Get();
      simpleFieldClass.SomeField.Should().Be("one");
   }

   [Fact]
   public void CreateSimpleFieldClassSomeOtherFieldIsSet() {
      var simpleFieldClass = GenerationSession.Single<SimpleFieldClass>().Get();
      simpleFieldClass.SomeOtherField.Should().Be("other");
   }

   [Fact]
   public void CreateUserEmailAddressIsSet() {
      var user = GenerationSession.Single<SimpleUser>().Get();
      user.EmailAddress.Should().Be("test@test.com");
   }

   [Fact]
   public void CreateUserFirstNameIsSet() {
      var user = GenerationSession.Single<SimpleUser>().Get();
      user.FirstName.Should().Be("first");
   }

   [Fact]
   public void CreateUserLastNameIsSet() {
      var user = GenerationSession.Single<SimpleUser>().Get();
      user.LastName.Should().Be("last");
   }
}