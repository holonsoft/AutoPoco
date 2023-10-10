using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DefaultFloatSourceTests {
   [Fact]
   public void NextReturnsZero() {
      var source = new DefaultFloatSource();
      var value = source.Next(null);
      value.Should().Be(0);
   }
}
