using Shouldly;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;
public class RandomUtfTextTests : TestBase {
   [Fact]
   public void NextReturnsAParagraph() {
      var source = new RandomUtfTextSource(256, 2, 3, 2, 3);
      var value = source.Next(null);

      value.ShouldNotBeNullOrWhiteSpace();
   }

   [Fact]
   public void NextTerminatesForManyDrawsEvenWhenABlockHasNoAllowedCharacters() {
      // regression: a block made only of excluded categories used to loop forever
      var source = new RandomUtfTextSource();

      var values = Enumerable.Range(0, 60).Select(_ => source.Next(null)).ToList();

      values.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x));
   }
}