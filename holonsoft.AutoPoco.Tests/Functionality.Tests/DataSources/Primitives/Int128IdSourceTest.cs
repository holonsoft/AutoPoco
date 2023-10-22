using holonsoft.AutoPoco.DataSources.Primitives;
using FluentAssertions;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class Int128IdSourceTests {
   [Fact]
   public void NextReturnsIncrementalResults() {
      var source = new Int128IdSource(0);
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().BeGreaterThan(value1);

      source = new Int128IdSource(10000);
      value1 = source.Next(null);
      value2 = source.Next(null);

      value1.Should().BeGreaterThan(9999);
      value2.Should().BeGreaterThan(value1);
   }
}