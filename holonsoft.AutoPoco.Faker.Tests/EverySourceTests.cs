using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Faker.DataSources;

namespace holonsoft.AutoPoco.Faker.Tests;

/// <summary>
///   The promises that hold for every source this package ships, checked on all of them instead of on one.
///   A source added later only has to be listed in <see cref="AllSources" /> to be covered.
/// </summary>
public class EverySourceTests {
   private static readonly (string Name, Func<DataSourceBase<string>> Create)[] _stringSources = [
      ("FirstName", () => new FakerFirstNameSource()),
      ("LastName", () => new FakerLastNameSource()),
      ("FullName", () => new FakerFullNameSource()),
      ("EmailAddress", () => new FakerEmailAddressSource()),
      ("PhoneNumber", () => new FakerPhoneNumberSource()),
      ("StreetAddress", () => new FakerStreetAddressSource()),
      ("City", () => new FakerCitySource()),
      ("Country", () => new FakerCountrySource()),
      ("PostalCode", () => new FakerPostalCodeSource()),
      ("CompanyName", () => new FakerCompanyNameSource()),
      ("ProductName", () => new FakerProductNameSource()),
      ("CurrencyCode", () => new FakerCurrencyCodeSource())
   ];

   public static TheoryData<string> AllSources() {
      var data = new TheoryData<string>();
      foreach (var (name, _) in _stringSources)
         data.Add(name);

      return data;
   }

   private static DataSourceBase<string> Seeded(string name, int seed) {
      var source = _stringSources.Single(s => s.Name == name).Create();
      source.SetSeedToRandomValue(seed);
      return source;
   }

   private static List<string> Draw(DataSourceBase<string> source, int count = 25)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Theory]
   [MemberData(nameof(AllSources))]
   public void TheSameSeedProducesTheSameValues(string name)
      => Draw(Seeded(name, 4711)).ShouldBe(Draw(Seeded(name, 4711)));

   [Theory]
   [MemberData(nameof(AllSources))]
   public void DifferentSeedsProduceDifferentValues(string name)
      => Draw(Seeded(name, 1)).ShouldNotBe(Draw(Seeded(name, 2)));

   [Theory]
   [MemberData(nameof(AllSources))]
   public void AValueIsNeverNullOrEmpty(string name)
      => Draw(Seeded(name, 7), 200).ShouldAllBe(v => !string.IsNullOrWhiteSpace(v));

   /// <summary>
   ///   A source that returned the same value over and over would pass every seed test above.
   /// </summary>
   [Theory]
   [MemberData(nameof(AllSources))]
   public void ASourceDoesNotRepeatOneValueForever(string name)
      => Draw(Seeded(name, 7), 200).Distinct().Count().ShouldBeGreaterThan(1);

   [Fact]
   public void ThePriceSourceStaysInsideItsBoundsAndRounds() {
      var source = new FakerPriceSource(10m, 20m, 2);
      source.SetSeedToRandomValue(4711);

      foreach (var value in Enumerable.Range(0, 200).Select(_ => source.Next(null))) {
         value.ShouldBeInRange(10m, 20m);
         value.ShouldBe(Math.Round(value, 2));
      }
   }

   [Fact]
   public void ThePriceSourceIsRepeatable() {
      var first = new FakerPriceSource(1m, 500m, 2);
      first.SetSeedToRandomValue(5);
      var second = new FakerPriceSource(1m, 500m, 2);
      second.SetSeedToRandomValue(5);

      Enumerable.Range(0, 50).Select(_ => first.Next(null))
         .ShouldBe(Enumerable.Range(0, 50).Select(_ => second.Next(null)));
   }

   [Fact]
   public void APriceRangeWithTheMaximumBelowTheMinimumIsRejected() {
      var ex = Should.Throw<ArgumentOutOfRangeException>(() => new FakerPriceSource(100m, 10m));
      ex.ParamName.ShouldBe("max");
   }

   [Theory]
   [InlineData(-1)]
   [InlineData(29)]
   public void AnImpossibleNumberOfDecimalsIsRejected(int decimals) {
      var ex = Should.Throw<ArgumentOutOfRangeException>(() => new FakerPriceSource(1m, 10m, decimals));
      ex.ParamName.ShouldBe("decimals");
   }

   [Fact]
   public void TheDateSourceStaysInsideItsBounds() {
      var min = new DateTime(2020, 1, 1);
      var max = new DateTime(2020, 12, 31);
      var source = new FakerDateTimeSource(min, max);
      source.SetSeedToRandomValue(4711);

      foreach (var value in Enumerable.Range(0, 200).Select(_ => source.Next(null)))
         value.ShouldBeInRange(min, max);
   }

   /// <summary>
   ///   The default range is fixed, it must not be measured from the current time. A source that read the
   ///   clock would stop being repeatable tomorrow.
   /// </summary>
   [Fact]
   public void TheDefaultDateRangeDoesNotDependOnTheClock() {
      var source = new FakerDateTimeSource();
      source.SetSeedToRandomValue(4711);

      var values = Enumerable.Range(0, 200).Select(_ => source.Next(null)).ToList();

      // pinned, not compared against themselves: bounds computed from DateTime.Now would fail here
      FakerDateDefaults.Min.ShouldBe(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified));
      FakerDateDefaults.Max.ShouldBe(new DateTime(2035, 12, 31, 23, 59, 59, DateTimeKind.Unspecified));

      values.ShouldAllBe(v => v >= FakerDateDefaults.Min && v <= FakerDateDefaults.Max);

      // Date.Past or Date.Future would cluster the values around today instead of filling the range
      (values.Max() - values.Min()).TotalDays.ShouldBeGreaterThan(365 * 25);
   }

   [Fact]
   public void ADateRangeWithTheMaximumBeforeTheMinimumIsRejected() {
      var ex = Should.Throw<ArgumentOutOfRangeException>(
         () => new FakerDateTimeSource(new DateTime(2030, 1, 1), new DateTime(2020, 1, 1)));

      ex.ParamName.ShouldBe("max");
   }

   /// <summary>
   ///   A Nullable member is already asked about null in DataSourceBase.Next. A second question inside the
   ///   source would roughly double the share of nulls, so a threshold of 50 has to stay near one half and
   ///   not slide towards three quarters.
   /// </summary>
   [Fact]
   public void ANullableValueTypeSourceIsAskedAboutNullExactlyOnce() {
      var source = new NullableFakerPriceSource(50);
      source.SetSeedToRandomValue(4711);

      var values = Enumerable.Range(0, 4000).Select(_ => source.Next(null)).ToList();
      var nullShare = values.Count(v => v == null) / (double) values.Count;

      nullShare.ShouldBeInRange(0.40, 0.60);
   }

   [Fact]
   public void ANullableReferenceTypeSourceHonoursItsThreshold() {
      var source = new NullableFakerCitySource(50);
      source.SetSeedToRandomValue(4711);

      var values = Enumerable.Range(0, 4000).Select(_ => source.Next(null)).ToList();
      var nullShare = values.Count(v => v == null) / (double) values.Count;

      nullShare.ShouldBeInRange(0.40, 0.60);
   }
}
