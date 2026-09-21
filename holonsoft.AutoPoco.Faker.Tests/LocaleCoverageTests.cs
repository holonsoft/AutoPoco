using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Faker.DataSources;
using BogusDatabase = Bogus.Database;

namespace holonsoft.AutoPoco.Faker.Tests;

/// <summary>
///   A Bogus locale does not have to carry every data set, and a hole shows up on the first draw, not when
///   the source is created. These tests walk every source this package ships against every locale of the
///   pinned Bogus version, so such a hole shows up here and not in someone's database run.
///   As of the pinned version every locale answers every source, missing data falls back to English rather
///   than throwing. The list is read from Bogus instead of being written out, so a locale that arrives with
///   a future version is covered the moment the pin moves.
/// </summary>
public class LocaleCoverageTests {
   /// <summary>
   ///   Every locale the pinned Bogus version carries. Each one is walked against every source below.
   /// </summary>
   public static readonly string[] SupportedLocales = [.. BogusDatabase.GetAllLocales().OrderBy(l => l)];

   private static IEnumerable<(string Name, Func<string, DataSourceBase<string>> Create)> StringSources() {
      yield return ("FirstName", l => new FakerFirstNameSource(l));
      yield return ("LastName", l => new FakerLastNameSource(l));
      yield return ("FullName", l => new FakerFullNameSource(l));
      yield return ("EmailAddress", l => new FakerEmailAddressSource(l));
      yield return ("PhoneNumber", l => new FakerPhoneNumberSource(l));
      yield return ("StreetAddress", l => new FakerStreetAddressSource(l));
      yield return ("City", l => new FakerCitySource(l));
      yield return ("Country", l => new FakerCountrySource(l));
      yield return ("PostalCode", l => new FakerPostalCodeSource(l));
      yield return ("CompanyName", l => new FakerCompanyNameSource(l));
      yield return ("ProductName", l => new FakerProductNameSource(l));
      yield return ("CurrencyCode", l => new FakerCurrencyCodeSource(l));
   }

   public static TheoryData<string> Locales() {
      var data = new TheoryData<string>();
      foreach (var locale in SupportedLocales)
         data.Add(locale);

      return data;
    }

   [Theory]
   [MemberData(nameof(Locales))]
   public void EveryStringSourceProducesAValueInEverySupportedLocale(string locale) {
      var problems = new List<string>();

      foreach (var (name, create) in StringSources()) {
         try {
            var source = create(locale);
            source.SetSeedToRandomValue(4711);

            for (var i = 0; i < 25; i++) {
               var value = source.Next(null);
               if (string.IsNullOrWhiteSpace(value)) {
                  problems.Add($"{name} produced an empty value");
                  break;
               }
            }
         } catch (Exception e) {
            problems.Add($"{name} threw {e.GetType().Name}: {e.Message}");
         }
      }

      problems.ShouldBeEmpty($"locale '{locale}' has holes: {string.Join(" | ", problems)}");
   }

   [Theory]
   [MemberData(nameof(Locales))]
   public void TheValueSourcesWorkInEverySupportedLocale(string locale) {
      var price = new FakerPriceSource(1m, 100m, 2, locale);
      price.SetSeedToRandomValue(4711);

      var date = new FakerDateTimeSource(FakerDateDefaults.Min, FakerDateDefaults.Max, locale);
      date.SetSeedToRandomValue(4711);

      for (var i = 0; i < 25; i++) {
         price.Next(null).ShouldBeInRange(1m, 100m);
         date.Next(null).ShouldBeInRange(FakerDateDefaults.Min, FakerDateDefaults.Max);
      }
   }

   /// <summary>
   ///   The theories above are driven by what Bogus reports. An empty or tiny list would make every one of
   ///   them pass without testing anything, so the list itself needs a floor, and the default locale has to
   ///   be part of it.
   /// </summary>
   [Fact]
   public void TheLocaleListIsReadFromBogusAndIsNotEmpty() {
      SupportedLocales.Length.ShouldBeGreaterThan(20);
      SupportedLocales.ShouldContain(FakerDefaults.Locale);
      SupportedLocales.ShouldContain("de");
      SupportedLocales.ShouldContain("de_AT");
      SupportedLocales.ShouldContain("de_CH");
   }

   /// <summary>
   ///   The source dimension of the theories above is an inner loop, not theory data, so an empty list would
   ///   let every locale pass without a single draw. The count is the floor under that.
   /// </summary>
   [Fact]
   public void EveryStringSourceOfThePackageIsInTheList()
      => StringSources().Count().ShouldBe(12);
}
