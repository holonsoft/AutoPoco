using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class DateOfBirthSourceTests : TestBase {
   private static List<DateTime> Draw(DateOfBirthSource source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Fact]
   public void NextReturnsAnAgeBetweenMinAndMax() {
      var source = new DateOfBirthSource(2015, 2018);
      var value = source.Next(null);

      value.Year.ShouldBeInRange(2015, 2018);
   }

   [Fact]
   public void NextReachesTheMaximumYearAndDecember31st() {
      var source = new DateOfBirthSource(2015, 2018);
      var values = Draw(source, 3000);

      values.ShouldAllBe(x => x.Year >= 2015 && x.Year <= 2018);
      values.ShouldContain(x => x.Year == 2018);
      values.ShouldContain(x => x.Year == 2015);
      values.ShouldContain(x => x.Month == 12 && x.Day == 31);
      values.ShouldContain(x => x.Month == 1 && x.Day == 1);
   }

   [Fact]
   public void NextWithASingleYearStaysInThatYear() {
      var source = new DateOfBirthSource(2024, 2024);
      var values = Draw(source, 2000);

      values.ShouldAllBe(x => x.Year == 2024);
      values.ShouldContain(new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Utc));
   }

   [Fact]
   public void NextReturnsUtcMidnight() {
      var values = Draw(new DateOfBirthSource(), 100);

      values.ShouldAllBe(x => x.Kind == DateTimeKind.Utc);
      values.ShouldAllBe(x => x.TimeOfDay == TimeSpan.Zero);
   }

   [Fact]
   public void NextThrowsWhenMaxYearIsBeforeMinYear() {
      var source = new DateOfBirthSource(2018, 2015);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   [Fact]
   public void SetMinMaxYearsChangesTheRange() {
      var source = new DateOfBirthSource();
      source.SetMinMaxYears(1980, 1981);

      Draw(source, 200).ShouldAllBe(x => x.Year == 1980 || x.Year == 1981);
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new DateOfBirthSource();
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime[] { new DateTime(609630624000000000, DateTimeKind.Utc), new DateTime(620042688000000000, DateTimeKind.Utc), new DateTime(613037376000000000, DateTimeKind.Utc), new DateTime(625907520000000000, DateTimeKind.Utc), new DateTime(621699840000000000, DateTimeKind.Utc), new DateTime(653166720000000000, DateTimeKind.Utc), new DateTime(646449984000000000, DateTimeKind.Utc), new DateTime(656324640000000000, DateTimeKind.Utc), new DateTime(652370112000000000, DateTimeKind.Utc), new DateTime(611629920000000000, DateTimeKind.Utc) });
   }

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableDateOfBirthSource();
      NextReturnsStableElementListInTermsOfTestability(source, new DateTime?[] { new DateTime(609630624000000000, DateTimeKind.Utc), new DateTime(620042688000000000, DateTimeKind.Utc), new DateTime(613037376000000000, DateTimeKind.Utc), new DateTime(625907520000000000, DateTimeKind.Utc), new DateTime(621699840000000000, DateTimeKind.Utc), new DateTime(653166720000000000, DateTimeKind.Utc), new DateTime(646449984000000000, DateTimeKind.Utc), new DateTime(656324640000000000, DateTimeKind.Utc), new DateTime(652370112000000000, DateTimeKind.Utc), new DateTime(611629920000000000, DateTimeKind.Utc) });
   }
}
