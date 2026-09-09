using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class LoremIpsumTests : TestBase {
   [Fact]
   public void NextReturnsAParagraph() {
      var source = new LoremIpsumSource();
      var value = source.Next(null);

      value.ShouldNotBeNullOrWhiteSpace();
   }

   [Fact]
   public void NextReturnsTwoParagraph() {
      var source = new LoremIpsumSource();
      var source2 = new LoremIpsumSource(2);

      var value = source.Next(null);
      var value2 = source2.Next(null);

      value2.Length.ShouldBeGreaterThan(value.Length);
   }

   [Fact]
   public void NextReturnsSometimesNull() {
      var source = new NullableLoremIpsumSource();

      // this works for default nullCreationThreshold!
      source.Next(null).ShouldNotBeNull();

      var loopProtectionCounter = 0;

      while (true) {
         var result = source.Next(null);
         if (result == null)
            break;

         loopProtectionCounter++;

         // 15 percent per draw: no null in 200 draws has a probability below 1e-13
         if (loopProtectionCounter > 200)
            throw new Exception($"Expected to get a result NULL, but did not occur the last {loopProtectionCounter} times");
      }
   }
}
