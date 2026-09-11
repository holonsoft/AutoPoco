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
   public void NextReachesTheMaximumParagraphCount() {
      // no truncation, so every paragraph survives and can be counted
      var source = new RandomUtfTextSource(int.MaxValue, 1, 3, 1, 1);
      var counts = Enumerable.Range(0, 200)
         .Select(_ => source.Next(null).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length)
         .ToList();

      counts.ShouldAllBe(c => c >= 1 && c <= 3);
      counts.ShouldContain(3);
      counts.ShouldContain(1);
   }

   [Fact]
   public void NextTerminatesForManyDrawsEvenWhenABlockHasNoAllowedCharacters() {
      // regression: a block made only of excluded categories used to loop forever
      var source = new RandomUtfTextSource();

      var values = Enumerable.Range(0, 60).Select(_ => source.Next(null)).ToList();

      values.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x));
   }
}