using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class FlexibleEnumerableSourceTests {
   [Fact]
   public void NextReturnsAFlexibleEnumrationOfAHundred() {
      var source = new FlexibleEnumerableSource<RandomStringSource, List<string>, string>(100, 100, new object[] { 4, 20 });
      var value = source.Next(null).ToArray();

      value.Should().HaveCount(100);
   }
}