using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class EnumerableSourceTests {
   [Fact]
   public void NextReturnsAEnumrationOfAHundred() {
      var source = new EnumerableSource<RandomStringSource, string>(100, new object[] { 4, 20 });
      var value = source.Next(null);

      value.Should().HaveCount(100);
   }
}
