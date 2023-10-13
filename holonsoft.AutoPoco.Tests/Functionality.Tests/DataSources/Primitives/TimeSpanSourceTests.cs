using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeSpanSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestability() {
      var source = new TimeSpanSource(new TimeSpan(10000000), new TimeSpan(10000000000));
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(source, new[] {
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
   }

   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeSpanSource(new TimeSpan(10000000), new TimeSpan(10000000000));

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan?[] {
            TimeSpan.FromTicks(2096794524),
            null,
            TimeSpan.FromTicks(1188990604),
            TimeSpan.FromTicks(3156509919),
            TimeSpan.FromTicks(9314280247),
            TimeSpan.FromTicks(8426247532),
            TimeSpan.FromTicks(9602920768),
            TimeSpan.FromTicks(9578739100),
            TimeSpan.FromTicks(1950302124),
            TimeSpan.FromTicks(3873914285),
         });
   }
}