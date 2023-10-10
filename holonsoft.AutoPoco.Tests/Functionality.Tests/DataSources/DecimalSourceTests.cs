using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DecimalSourceTests {
   [Fact]
   public void NextReturnsNotZero() {
      var source = new DecimalSource();
      var value = source.Next(null);
      value.Should().NotBe(0);
   }
}
