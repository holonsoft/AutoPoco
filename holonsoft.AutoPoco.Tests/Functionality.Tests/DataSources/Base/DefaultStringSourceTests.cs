using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class DefaultStringSourceTests {
   [Fact]
   public void NextReturnsEmptyString() {
      var source = new DefaultStringSource();
      var value = source.Next(null);
      value.Should().BeEmpty();
   }
}
