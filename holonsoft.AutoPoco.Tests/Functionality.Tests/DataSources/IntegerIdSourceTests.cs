using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class IntegerIdSourceTests {
   [Fact]
   public void NextReturnsIncrementalResults() {
      var source = new IntegerIdSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().BeGreaterThan(value1);
   }
}
