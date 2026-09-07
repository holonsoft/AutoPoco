using Shouldly;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests;

public class AutoPocoContainerTests {
   [Fact]
   public void ConfigureRunsActions() {
      var hasRun = false;
      AutoPocoContainer.Configure(x => { hasRun = true; });
      hasRun.ShouldBeTrue();
   }

   [Fact]
   public void ConfigureReturnsFactory() {
      var factory = AutoPocoContainer.Configure(x => { });
      factory.ShouldNotBeNull();
   }
}