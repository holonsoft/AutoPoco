using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateTimeSourceTests : TestBase {
   private readonly DateTime _minDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   private readonly DateTime _maxDate = new(2030, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

   private static List<DateTime> Draw(DateTimeSource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Fact]
   public void NextReturnsDateTimeBetweenMinAndMax() {
      var source = new DateTimeSource(_minDate, _maxDate);
      var value = source.Next(null);

      value.ShouldBeInRange(_minDate, _maxDate);
   }

   [Fact]
   public void NextStaysInRangeOverManyDraws() {
      var source = new DateTimeSource(_minDate, _maxDate);

      Draw(source, 2000).ShouldAllBe(x => x >= _minDate && x <= _maxDate);
   }

   [Fact]
   public void NextReachesDecemberTheLastDayAndTheLastHourMinuteAndSecond() {
      var source = new DateTimeSource(_minDate, _maxDate);
      var values = Draw(source, 3000);

      values.ShouldContain(x => x.Month == 12);
      values.ShouldContain(x => x.Day == 31);
      values.ShouldContain(x => x.Hour == 23);
      values.ShouldContain(x => x.Minute == 59);
      values.ShouldContain(x => x.Second == 59);
      values.ShouldContain(x => x.Year == _maxDate.Year);
   }

   [Fact]
   public void NextStaysInRangeForARangeWithinOneYear() {
      var min = new DateTime(2030, 3, 1, 0, 0, 0, DateTimeKind.Utc);
      var max = new DateTime(2030, 3, 31, 23, 59, 59, DateTimeKind.Utc);
      var source = new DateTimeSource(min, max);

      var values = Draw(source, 500);

      values.ShouldAllBe(x => x >= min && x <= max);
      values.ShouldContain(x => x.Day == 31);
      values.ShouldContain(x => x.Day == 1);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatInstant() {
      var instant = new DateTime(2024, 2, 29, 12, 34, 56, DateTimeKind.Utc);
      var source = new DateTimeSource(instant, instant);

      Draw(source, 10).ShouldAllBe(x => x == instant);
   }

   [Fact]
   public void NextKeepsTheKindOfTheMinimumDate() {
      new DateTimeSource(_minDate, _maxDate).Next(null).Kind.ShouldBe(DateTimeKind.Utc);
      new DateTimeSource(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Local))
         .Next(null).Kind.ShouldBe(DateTimeKind.Local);
   }

   [Fact]
   public void NextWorksForTheFullDateTimeRange() {
      var source = new DateTimeSource();

      Draw(source, 100).ShouldAllBe(x => x >= DateTime.MinValue && x <= DateTime.MaxValue);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBeforeMin() {
      var source = new DateTimeSource(_maxDate, _minDate);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetDateRangeChangesTheRange() {
      var min = new DateTime(2010, 6, 1, 0, 0, 0, DateTimeKind.Utc);
      var max = new DateTime(2010, 6, 2, 0, 0, 0, DateTimeKind.Utc);
      var source = new DateTimeSource(_minDate, _maxDate);

      source.SetDateRange(min, max);

      Draw(source, 100).ShouldAllBe(x => x >= min && x <= max);
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateTimeSource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime[] {
         new DateTime(636496727523570519, DateTimeKind.Utc),
         new DateTime(637790390162231123, DateTimeKind.Utc),
         new DateTime(635912374590276787, DateTimeKind.Utc),
         new DateTime(639196215649992172, DateTimeKind.Utc),
         new DateTime(632769569224965583, DateTimeKind.Utc)
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateTimeSource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability<DateTime?>(source, new DateTime?[] {
         new DateTime(636496727523570519, DateTimeKind.Utc),
         null,
         new DateTime(637790390162231123, DateTimeKind.Utc),
         new DateTime(635912374590276787, DateTimeKind.Utc),
         new DateTime(639196215649992172, DateTimeKind.Utc)
      });
   }
}
