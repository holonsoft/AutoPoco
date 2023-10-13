using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class LoremIpsumTests : TestBase {
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

   [Fact]
   public void NextReturnsSometimesNull() {
      var source = new NullableLoremIpsumSource();

      // this works for default nullCreationThreshold!
      source.Next(null).Should().NotBeNull();

      var loopProtectionCounter = 0;

      while (true) {
         var result = source.Next(null);
         if (result == null)
            break;

         loopProtectionCounter++;

         if (loopProtectionCounter > 20)
            throw new Exception($"Expceted to get a result NULL, but did not occure the last {loopProtectionCounter} times");
      }
   }
}
