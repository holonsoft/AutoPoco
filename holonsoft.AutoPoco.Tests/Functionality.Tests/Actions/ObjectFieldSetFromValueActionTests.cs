using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Actions;

public class ObjectFieldSetFromValueActionTests {
   [Fact]
   public void EnactSetsFieldWithValue() {
      var action = new ObjectFieldSetFromValueAction((EngineTypeFieldMember)
        ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!), "Test");

      var target = new SimpleFieldClass();
      action.Enact(null, target);

      target.SomeField.Should().Be("Test");
   }
}