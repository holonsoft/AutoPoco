using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class LoremIpsumTests {
   [Fact]
   public void NextReturnsAParagraph() {
      var source = new LoremIpsumSource();
      var value = source.Next(null);

      value.Should().NotBeNullOrWhiteSpace();
   }

   [Fact]
   public void NextReturnsTwoParagraph() {
      var source = new LoremIpsumSource();
      var source2 = new LoremIpsumSource(2);

      var value = source.Next(null);
      var value2 = source2.Next(null);

      value2.Length.Should().BeGreaterThan(value.Length);
   }
}