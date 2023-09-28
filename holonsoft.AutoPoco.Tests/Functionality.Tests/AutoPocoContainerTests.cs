using FluentAssertions;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests;

public class AutoPocoContainerTests {
   [Fact]
   public void ConfigureRunsActions() {
      var hasRun = false;
      AutoPocoContainer.Configure(x => { hasRun = true; });
      hasRun.Should().BeTrue();
   }

   [Fact]
   public void ConfigureReturnsFactory() {
      var factory = AutoPocoContainer.Configure(x => { });
      factory.Should().NotBeNull();
   }
}