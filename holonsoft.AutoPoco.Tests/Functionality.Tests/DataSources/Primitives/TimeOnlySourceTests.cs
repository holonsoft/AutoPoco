using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeOnlySourceTests : TestBase {
   private readonly TimeOnly _minDate = new(8, 0, 0);
   private readonly TimeOnly _maxDate = new(17, 0, 0);

   private static List<TimeOnly> Draw(TimeOnlySource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Fact]
   public void NextReturnsDateBetweenMinAndMax() {
      var source = new TimeOnlySource(_minDate, _maxDate);
      var value = source.Next(null);

      value.ShouldBeInRange(_minDate, _maxDate);
   }

   [Fact]
   public void NextStaysInRangeOverManyDraws() {
      var source = new TimeOnlySource(_minDate, _maxDate);

      Draw(source, 2000).ShouldAllBe(x => x >= _minDate && x <= _maxDate);
   }

   [Fact]
   public void NextReachesTheLastHourMinuteAndSecondOfTheDay() {
      var source = new TimeOnlySource();
      var values = Draw(source, 3000);

      values.ShouldContain(x => x.Hour == 23);
      values.ShouldContain(x => x.Minute == 59);
      values.ShouldContain(x => x.Second == 59);
      values.ShouldContain(x => x.Hour == 0);
   }

   [Fact]
   public void NextReachesTheUpperBoundOfAShortRange() {
      // a 10 millisecond window: missing millisecond 999 in 300 draws has a probability of 0.9^300
      var min = new TimeOnly(23, 59, 59, 990);
      var max = TimeOnly.MaxValue;
      var source = new TimeOnlySource(min, max);
      var values = Draw(source, 300);

      values.ShouldAllBe(x => x >= min && x <= max);
      values.ShouldContain(x => x.Millisecond == 999);
      values.ShouldContain(x => x.Millisecond == 990);
   }

   [Fact]
   public void NextWrapsAroundMidnightWhenMaxIsBeforeMin() {
      var source = new TimeOnlySource(new TimeOnly(22, 0, 0), new TimeOnly(2, 0, 0));
      var values = Draw(source, 500);

      values.ShouldAllBe(x => x.Hour >= 22 || x.Hour < 2 || x == new TimeOnly(2, 0, 0));
      values.ShouldContain(x => x.Hour >= 22);
      values.ShouldContain(x => x.Hour < 2);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatTime() {
      var time = new TimeOnly(12, 34, 56, 789);
      var source = new TimeOnlySource(time, time);

      Draw(source, 10).ShouldAllBe(x => x == time);
   }

   [Fact]
   public void SetMinMaxRangeChangesTheRange() {
      var source = new TimeOnlySource(_minDate, _maxDate);
      source.SetMinMaxRange(new TimeOnly(10, 0, 0), new TimeOnly(10, 0, 1));

      Draw(source, 100).ShouldAllBe(x => x >= new TimeOnly(10, 0, 0) && x <= new TimeOnly(10, 0, 1));
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new TimeOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new TimeOnly[] { new TimeOnly(426772885212), new TimeOnly(593448500803), new TimeOnly(371913204116), new TimeOnly(328551510961), new TimeOnly(364295624019) });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new TimeOnly?[] { new TimeOnly(426772885212), new TimeOnly(593448500803), new TimeOnly(371913204116), new TimeOnly(328551510961), new TimeOnly(364295624019) });
   }
}
