using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateOnlySourceTests : TestBase {
   private readonly DateOnly _minDate = new(2000, 1, 1);
   private readonly DateOnly _maxDate = new(2030, 12, 31);

   private static List<DateOnly> Draw(DateOnlySource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Fact]
   public void NextReturnsDateBetweenMinAndMax() {
      var source = new DateOnlySource(_minDate, _maxDate);
      var value = source.Next(null);

      value.ShouldBeInRange(_minDate, _maxDate);
   }

   [Fact]
   public void NextStaysInRangeOverManyDraws() {
      var source = new DateOnlySource(_minDate, _maxDate);

      Draw(source, 2000).ShouldAllBe(x => x >= _minDate && x <= _maxDate);
   }

   [Fact]
   public void NextReachesDecemberAndTheLastDayOfAMonth() {
      var source = new DateOnlySource(_minDate, _maxDate);
      var values = Draw(source, 2000);

      values.ShouldContain(x => x.Month == 12);
      values.ShouldContain(x => x.Day == 31);
      values.ShouldContain(x => x.Year == _maxDate.Year);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortRange() {
      var min = new DateOnly(2020, 12, 25);
      var max = new DateOnly(2020, 12, 31);
      var source = new DateOnlySource(min, max);
      var values = Draw(source, 300);

      values.ShouldAllBe(x => x >= min && x <= max);
      values.ShouldContain(min);
      values.ShouldContain(max);
   }

   [Fact]
   public void NextReachesFebruary29InALeapYear() {
      var source = new DateOnlySource(new DateOnly(2024, 2, 28), new DateOnly(2024, 2, 29));

      Draw(source, 50).ShouldContain(new DateOnly(2024, 2, 29));
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatDay() {
      var day = new DateOnly(2024, 2, 29);
      var source = new DateOnlySource(day, day);

      Draw(source, 10).ShouldAllBe(x => x == day);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBeforeMin() {
      var source = new DateOnlySource(_maxDate, _minDate);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetMinAndMaxDateChangeTheRange() {
      var source = new DateOnlySource(_minDate, _maxDate);
      source.SetMinDate(new DateOnly(2010, 5, 1)).SetMaxDate(new DateOnly(2010, 5, 3));

      Draw(source, 100).ShouldAllBe(x => x >= new DateOnly(2010, 5, 1) && x <= new DateOnly(2010, 5, 3));
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateOnly[] {
         new DateOnly(2006, 6, 23),
         new DateOnly(2003, 8, 29),
         new DateOnly(2009, 10, 6),
         new DateOnly(2028, 11, 14),
         new DateOnly(2026, 2, 12)
      });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateOnlySource(_minDate, _maxDate);
      NextReturnsStableElementListInTermsOfTestability(source, new DateOnly?[] {
         new DateOnly(2006, 6, 23),
         null,
         new DateOnly(2003, 8, 29),
         new DateOnly(2009, 10, 6),
         new DateOnly(2028, 11, 14)
      });
   }
}
