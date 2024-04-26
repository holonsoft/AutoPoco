using FluentAssertions;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;
public class RandomUtfTextTests : TestBase {
   [Fact]
   public void NextReturnsAParagraph() {
      var source = new RandomUtfTextSource(256, 2, 3, 2, 3);
      var value = source.Next(null);

      value.Should().NotBeNullOrWhiteSpace();
   }
}