﻿using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenMultipleConventionsApplyToSameProperty {
   [Fact]
   public void TypeOverridesNone() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => {
            c.Register<DefaultTypeConvention>();
            c.Register<SetPropertiesOfStringTo1>();
            c.Register<SetPropertiesTo0>();
         });
         x.Include<TestMultiplePropertyConventionClass>();
      })
        .CreateSession();

      var testObj
        = session.Single<TestMultiplePropertyConventionClass>().Get();

      Assert.True(
        testObj.FirstIntegerProperty == 0 &&
        testObj.FirstStringProperty == "1" &&
        testObj.SecondIntegerProperty == 0 &&
        testObj.SecondStringProperty == "1", "Conventions were not applied in expected order"
      );
   }

   [Fact]
   public void NameOverridesType() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => {
            c.Register<DefaultTypeConvention>();
            c.Register<SetPropertiesContainingFirstTo2>();
            c.Register<SetPropertiesOfStringTo1>();
            c.Register<SetPropertiesTo0>();
         });
         x.Include<TestMultiplePropertyConventionClass>();
      })
        .CreateSession();

      var testObj
        = session.Single<TestMultiplePropertyConventionClass>().Get();

      Assert.True(
        testObj.FirstIntegerProperty == 2 &&
        testObj.FirstStringProperty == "2" &&
        testObj.SecondIntegerProperty == 0 &&
        testObj.SecondStringProperty == "1", "Conventions were not applied in expected order"
      );
   }

   [Fact]
   public void NameAndTypeOverrideName() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => {
            c.Register<DefaultTypeConvention>();
            c.Register<SetPropertiesOfStringCalledFirstTo3>();
            c.Register<SetPropertiesContainingFirstTo2>();
            c.Register<SetPropertiesOfStringTo1>();
            c.Register<SetPropertiesTo0>();
         });
         x.Include<TestMultiplePropertyConventionClass>();
      })
        .CreateSession();

      var testObj
        = session.Single<TestMultiplePropertyConventionClass>().Get();

      Assert.True(
        testObj.FirstIntegerProperty == 2 &&
        testObj.FirstStringProperty == "3" &&
        testObj.SecondIntegerProperty == 0 &&
        testObj.SecondStringProperty == "1", "Conventions were not applied in expected order"
      );
   }

   public class SetPropertiesTo0 : ITypePropertyConvention {
      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) {
      }

      public void Apply(ITypePropertyConventionContext context) {
         if (context.Member.PropertyInfo.PropertyType == typeof(string))
            context.SetValue("0");
         else if (context.Member.PropertyInfo.PropertyType == typeof(int?))
            context.SetValue(0);
      }
   }

   public class SetPropertiesOfStringTo1 : ITypePropertyConvention {
      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Type(x => x == typeof(string));

      public void Apply(ITypePropertyConventionContext context) => context.SetValue("1");
   }

   public class SetPropertiesContainingFirstTo2 : ITypePropertyConvention {
      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Name(x => x.StartsWith("First"));

      public void Apply(ITypePropertyConventionContext context) {
         if (context.Member.PropertyInfo.PropertyType == typeof(string))
            context.SetValue("2");
         else if (context.Member.PropertyInfo.PropertyType == typeof(int?))
            context.SetValue(2);
      }
   }

   public class SetPropertiesOfStringCalledFirstTo3 : ITypePropertyConvention {
      public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) {
         requirements.Name(x => x.StartsWith("First"));
         requirements.Type(x => x == typeof(string));
      }

      public void Apply(ITypePropertyConventionContext context) => context.SetValue("3");
   }

   public class TestMultiplePropertyConventionClass {
      public int? FirstIntegerProperty { get; set; }

      public int? SecondIntegerProperty { get; set; }

      public required string FirstStringProperty { get; set; }

      public required string SecondStringProperty { get; set; }
   }
}