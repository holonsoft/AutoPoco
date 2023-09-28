using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public abstract class SomeFoo {
   public abstract string Something { get; protected set; }
}

public class Foo : SomeFoo {
   public override string Something {
      get => throw new NotImplementedException();
      protected set => Console.WriteLine("Blah");
   }
}

public class TypeMemberConventionRequirementsTests {
   [Theory]
   [InlineData("EmailAddress", true)]
   [InlineData("hello", false)]
   [InlineData("2", false)]
   public void ApplyPropertyNameRuleRuleIsApplied(string test, bool result) {
      var context = new TypePropertyConventionRequirements();
      context.Name(x => x == test);
      var member = (EngineTypePropertyMember) ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);
      context.IsValid(member).Should().Be(result);
   }

   [Theory]
   [InlineData("SomeField", true)]
   [InlineData("hello", false)]
   [InlineData("2", false)]
   public void ApplyFieldNameRuleRuleIsApplied(string test, bool result) {
      var context = new TypeFieldConventionRequirements();
      context.Name(x => x == test);
      var member = (EngineTypeFieldMember) ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!);

      context.IsValid(member).Should().Be(result);
      //Assert.AreEqual(result, context.IsValid(member));
   }

   [Theory]
   [InlineData(typeof(string), true)]
   [InlineData(typeof(bool), false)]
   [InlineData(typeof(SimpleUser), false)]
   public void ApplyPropertyTypeRuleRuleIsApplied(Type test, bool result) {
      var context = new TypePropertyConventionRequirements();
      context.Type(x => x == test);
      var member = (EngineTypePropertyMember) ReflectionHelper.GetMember<SimpleUser>(x => x.EmailAddress);

      context.IsValid(member).Should().Be(result);

      //Assert.AreEqual(result, context.IsValid(member));
   }

   [Theory]
   [InlineData(typeof(string), true)]
   [InlineData(typeof(bool), false)]
   [InlineData(typeof(SimpleUser), false)]
   public void ApplyFieldTypeRuleRuleIsApplied(Type test, bool result) {
      var context = new TypeFieldConventionRequirements();
      context.Type(x => x == test);
      var member = (EngineTypeFieldMember) ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!);
      context.IsValid(member).Should().Be(result);
      //Assert.AreEqual(result, context.IsValid(member));
   }
}