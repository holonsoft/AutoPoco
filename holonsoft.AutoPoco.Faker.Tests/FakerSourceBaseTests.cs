using Bogus;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Faker.DataSources;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.Tests;

/// <summary>
///   The behaviour every Faker source inherits: seeding, isolation and locale handling. The tests assert
///   shape and repeatability, never a literal value. Bogus takes its values from locale data that ships
///   inside its package, so pinning literals here would make a Bogus update look like a defect.
/// </summary>
public class FakerSourceBaseTests {
   private static List<string> Draw(DataSourceBase<string> source, int count = 25)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   private static FakerFirstNameSource Seeded(int seed, string? locale = null) {
      var source = new FakerFirstNameSource(locale);
      source.SetSeedToRandomValue(seed);
      return source;
   }

   [Fact]
   public void TheSameSeedProducesTheSameValues()
      => Draw(Seeded(4711)).ShouldBe(Draw(Seeded(4711)));

   [Fact]
   public void DifferentSeedsProduceDifferentValues()
      => Draw(Seeded(1)).ShouldNotBe(Draw(Seeded(2)));

   /// <summary>
   ///   The point of the whole exercise. A Bogus randomizer built from an int seed runs on
   ///   <see cref="System.Random" />, which is deterministic as well, so every seed test in this file would
   ///   pass even if the injection of AutoPoco's StableRandom silently did nothing. This is the one test
   ///   that fails in that case: the same seed through plain Bogus has to give a different sequence.
   /// </summary>
   [Fact]
   public void TheSourceDrawsFromStableRandomAndNotFromSystemRandom() {
      const int seed = 4711;

      var throughAutoPoco = Draw(Seeded(seed));

      var plainBogus = new BogusFaker("en") { Random = new Randomizer(seed) };
      var throughSystemRandom = Enumerable.Range(0, throughAutoPoco.Count)
         .Select(_ => plainBogus.Name.FirstName())
         .ToList();

      throughAutoPoco.ShouldNotBe(throughSystemRandom,
         "the source appears to run on System.Random, so the StableRandomizer is not in place");
   }

   /// <summary>
   ///   Bogus keeps a static <c>Randomizer.Seed</c>. A source that built a plain <c>new Randomizer()</c>
   ///   somewhere inside would copy that instance and draw from it, so every source in the process would
   ///   share one stream. Comparing the reference alone does not catch that, the instance would still be the
   ///   same one; what gives it away is that the stream has moved on. So this asserts the position.
   /// </summary>
   [Fact]
   public void TheGlobalBogusRandomizerIsNeitherReadNorAdvanced() {
      var before = Randomizer.Seed;

      try {
         Randomizer.Seed = new Random(999);

         Draw(Seeded(99), 100);
         Draw(new FakerFullNameSource("de"), 100);
         Draw(new FakerEmailAddressSource(), 100);

         // a stream nobody drew from still delivers what a fresh one of the same seed delivers
         var untouched = new Random(999);
         Enumerable.Range(0, 5).Select(_ => Randomizer.Seed.Next()).ToList()
            .ShouldBe(Enumerable.Range(0, 5).Select(_ => untouched.Next()).ToList(),
               "the global Bogus randomizer has moved on, so something drew from it");
      } finally {
         Randomizer.Seed = before;
      }
   }

   /// <summary>
   ///   Two sources on the same seed must not influence each other, no matter in which order they are read.
   /// </summary>
   [Fact]
   public void TwoSourcesOnTheSameSeedDoNotShareState() {
      var first = Seeded(7);
      var second = Seeded(7);

      var interleaved = new List<string>();
      var straight = new List<string>();

      for (var i = 0; i < 25; i++) {
         interleaved.Add(first.Next(null));
         second.Next(null);
      }

      var third = Seeded(7);
      for (var i = 0; i < 25; i++)
         straight.Add(third.Next(null));

      interleaved.ShouldBe(straight);
   }

   [Fact]
   public void AValueIsNeverNullOrEmptyWithoutANullThreshold()
      => Draw(Seeded(3), 200).ShouldAllBe(v => !string.IsNullOrWhiteSpace(v));

   [Fact]
   public void TheLocaleDecidesTheValues() {
      var german = Draw(Seeded(5, "de"), 50);
      var english = Draw(Seeded(5, "en"), 50);

      german.ShouldNotBe(english);
      german.ShouldAllBe(v => !string.IsNullOrWhiteSpace(v));
   }

   [Fact]
   public void TheDefaultLocaleIsTheLibraryDefault()
      => new FakerFirstNameSource().Locale.ShouldBe(FakerDefaults.Locale);

   [Theory]
   [InlineData("klingon")]
   [InlineData("DE")]
   [InlineData("de-AT")]
   [InlineData("")]
   public void AnUnknownLocaleIsRejected(string locale) {
      var ex = Should.Throw<ArgumentException>(() => new FakerFirstNameSource(locale));
      ex.ParamName.ShouldBe("locale");
   }

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseRealValues() {
      var source = new NullableFakerFirstNameSource(50);
      source.SetSeedToRandomValue(11);

      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);
      values.Where(v => v != null).ShouldAllBe(v => !string.IsNullOrWhiteSpace(v));
   }

   [Fact]
   public void ANullableSourceKeepsTheLocale()
      => new NullableFakerFirstNameSource("de").Locale.ShouldBe("de");

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Draw(Seeded(13), 500).ShouldAllBe(v => v != null);

   /// <summary>
   ///   Reseeding has to reach the faker, not only the random stream of the base class. A source that built
   ///   its faker once in the constructor would repeat the same values after a reseed.
   /// </summary>
   [Fact]
   public void ReseedingReplacesTheFaker() {
      var source = new FakerFirstNameSource();
      var beforeReseed = Draw(source);

      source.SetSeedToRandomValue(20250921);
      var afterReseed = Draw(source);

      afterReseed.ShouldNotBe(beforeReseed);
   }
}
