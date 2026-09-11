using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeSpanSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestability() {
      var source = new TimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan[] { new TimeSpan(5193727380), new TimeSpan(-9554848653), new TimeSpan(-7502659219), new TimeSpan(-3808227407), new TimeSpan(-2423852717), new TimeSpan(2896633336), new TimeSpan(-4073448608), new TimeSpan(-918960852), new TimeSpan(-4345801754), new TimeSpan(8042706088) });
   }

   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan?[] { new TimeSpan(-8666068260), new TimeSpan(-2239989265), new TimeSpan(5193727380), new TimeSpan(-9554848653), new TimeSpan(-7502659219), new TimeSpan(-3808227407), new TimeSpan(-2423852717), new TimeSpan(2896633336), new TimeSpan(-4073448608), new TimeSpan(-918960852) });
   }

   [Fact]
   public void NextStaysInsideAPositiveRange() {
      // the old implementation returned 22 to 58 minutes for this range, every value outside it
      var min = TimeSpan.FromHours(1);
      var max = TimeSpan.FromHours(2);
      var source = new TimeSpanSource(min, max);

      Draw(source, 500).ShouldAllBe(x => x >= min && x <= max);
   }

   [Fact]
   public void NextVariesOverTheWholeRangeOfTheType() {
      // the old implementation returned TimeSpan.MinValue on every draw here
      var values = Draw(new TimeSpanSource(), 200);

      values.Distinct().Count().ShouldBe(200);
      values.ShouldContain(x => x > TimeSpan.Zero);
      values.ShouldContain(x => x < TimeSpan.Zero);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortRange() {
      var min = new TimeSpan(100);
      var max = new TimeSpan(102);
      var values = Draw(new TimeSpanSource(min, max), 300);

      values.ShouldAllBe(x => x >= min && x <= max);
      values.ShouldContain(min);
      values.ShouldContain(max);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      var span = TimeSpan.FromMinutes(7);

      Draw(new TimeSpanSource(span, span), 10).ShouldAllBe(x => x == span);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBeforeMin() {
      // the old implementation looped forever here
      var source = new TimeSpanSource(TimeSpan.FromHours(2), TimeSpan.FromHours(1));

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetMinMaxRangeChangesTheRange() {
      var source = new TimeSpanSource();
      source.SetMinMaxRange(TimeSpan.FromDays(1), TimeSpan.FromDays(2));

      Draw(source, 100).ShouldAllBe(x => x >= TimeSpan.FromDays(1) && x <= TimeSpan.FromDays(2));
   }

   private static List<TimeSpan> Draw(TimeSpanSource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();
}