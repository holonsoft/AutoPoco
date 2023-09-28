using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Actions;

public class ObjectMethodInvokeActionActionTests {
   [Fact]
   public void EnactSetsPropertyWithValue() {
      var action = new ObjectMethodInvokeActionAction<SimpleMethodClass>(x => x.SetSomething("Something"));
      var target = new SimpleMethodClass();
      action.Enact(null, target);

      target.Value.Should().Be("Something");
   }
}