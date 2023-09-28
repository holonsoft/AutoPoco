using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Actions;

public class ObjectPropertySetFromValueActionTests {
   [Fact]
   public void EnactSetsFieldWithValue() {
      var action = new ObjectPropertySetFromValueAction((EngineTypePropertyMember)
        ReflectionHelper.GetMember<SimplePropertyClass>(x => x.SomeProperty!), "Test");

      var target = new SimplePropertyClass();
      action.Enact(null, target);

      target.SomeProperty.Should().Be("Test");
   }
}