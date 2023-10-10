using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class ColorSourceTests {
   [Fact]
   public void NextReturnsAColor() {
      var source = new ColorSource();
      var value = source.Next(null);

      value.IsEmpty.Should().BeFalse();
   }
}
