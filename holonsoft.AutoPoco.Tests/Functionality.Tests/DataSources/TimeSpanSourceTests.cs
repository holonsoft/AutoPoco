using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class TimeSpanSourceTests {
   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestability() {
      var source = new TimeSpanSource(new TimeSpan(10000000), new TimeSpan(10000000000));
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      List<TimeSpan> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] {
            new TimeSpan(0, 0, 5, 15, 650) + TimeSpan.FromTicks(9919),
            new TimeSpan(0, 0, 15, 31, 428) + TimeSpan.FromTicks(247),
            new TimeSpan(0, 0, 14, 2, 624) + TimeSpan.FromTicks(7532),
            new TimeSpan(0, 0, 16, 0, 292) + TimeSpan.FromTicks(768),
            new TimeSpan(0, 0, 15, 57, 873) + TimeSpan.FromTicks(9100),
            new TimeSpan(0, 0, 3, 15, 30) + TimeSpan.FromTicks(2124),
            new TimeSpan(0, 0, 6, 27, 391) + TimeSpan.FromTicks(4285),
            new TimeSpan(0, 0, 4, 54, 628) + TimeSpan.FromTicks(833),
            new TimeSpan(0, 0, 5, 1, 321) + TimeSpan.FromTicks(5203),
            new TimeSpan(0, 0, 4, 43, 245) + TimeSpan.FromTicks(3776)
         });

      List<TimeSpan> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      List<TimeSpan> randomGenerated2 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10);

      randomGenerated1.Should().NotBeEquivalentTo(generated);
      randomGenerated2.Should().NotBeEquivalentTo(generated);
      randomGenerated1.Should().NotBeEquivalentTo(randomGenerated2);
   }
}