using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Actions;

public class ObjectMethodInvokeFuncActionTests {
   [Fact]
   public void EnactCallsFunc() {
      var action = new ObjectMethodInvokeFuncAction<SimpleMethodClass, string>(x => x.ReturnSomething());

      var target = new SimpleMethodClass();
      action.Enact(null, target);

      target.ReturnSomethingCalled.Should().BeTrue();
   }
}