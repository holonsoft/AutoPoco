using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class IntegerIdSourceTests {
   [Fact]
   public void NextReturnsIncrementalResults() {
      var source = new IntegerIdSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().BeGreaterThan(value1);

      source = new IntegerIdSource(10000);
      value1 = source.Next(null);
      value2 = source.Next(null);

      value1.Should().BeGreaterThan(9999);
      value2.Should().BeGreaterThan(value1);
   }
}