using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using BogusDatabase = Bogus.Database;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker;

/// <summary>
///   Base class of every data source that takes its value from Bogus. It owns one Bogus faker per source
///   instance and builds a new one whenever the engine hands down a seed, so the AutoPoco session seed
///   decides what Bogus produces.
/// </summary>
/// <typeparam name="T">The member type this source fills.</typeparam>
/// <remarks>
///   One faker per source instance, never a shared one. Every member of a generated object gets its own
///   source, so two members never pull from the same stream, and a second session starts from its own seed.
///   A single source instance is still single threaded, like every other AutoPoco source.
/// </remarks>
public abstract class FakerSourceBase<T> : DataSourceBase<T> {
   /// <summary>
   ///   Every locale the referenced Bogus version knows, looked up once. Ordinal on purpose, Bogus writes
   ///   its locale names lower case and does not treat them case insensitively.
   /// </summary>
   private static readonly HashSet<string> _knownLocales = new(BogusDatabase.GetAllLocales(), StringComparer.Ordinal);

   /// <summary>
   ///   True when this source decides about null inside <see cref="GetNextValue" />, which is the case for a
   ///   reference type. A <see cref="Nullable{T}" /> member is already asked in
   ///   <see cref="DataSourceBase{T}.Next" />, and asking a second time would roughly double the share of nulls.
   /// </summary>
   private static readonly bool _decidesAboutNullItself = !typeof(T).IsValueType;

   private readonly string _locale;

   private int _seed = AutoPocoDefaults.Seed;
   private BogusFaker? _faker;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="locale">A Bogus locale, <see cref="FakerDefaults.Locale" /> when null.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="locale" /> is not a locale Bogus knows.</exception>
   /// <exception cref="InvalidOperationException">
   ///   A null creation threshold was given although <typeparamref name="T" /> cannot hold a null.
   /// </exception>
   protected FakerSourceBase(string? locale = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      _locale = locale ?? FakerDefaults.Locale;

      if (!_knownLocales.Contains(_locale))
         throw new ArgumentException(
            $"'{_locale}' is not a locale Bogus knows. Locales are lower case with an underscore before the region, e.g. 'en', 'en_US', 'de', 'de_AT'.",
            nameof(locale));

      // without this a derived source over a plain value type would silently hand out default(T) instead of null
      if (nullCreationThreshold.HasValue && !_decidesAboutNullItself && Nullable.GetUnderlyingType(typeof(T)) == null)
         throw new InvalidOperationException(
            $"A null creation threshold needs a member that can hold a null, '{typeof(T).Name}' cannot. Use '{typeof(T).Name}?' instead.");
   }

   /// <summary>
   ///   The Bogus faker of this source, built on first use and again after every reseed. A derived source
   ///   reads it per value instead of caching it.
   /// </summary>
   protected BogusFaker Faker => _faker ??= CreateFaker(_seed);

   /// <summary>
   ///   The locale this source was created with.
   /// </summary>
   public string Locale => _locale;

   /// <summary>
   ///   Reseeds the source and drops the faker, so the next value comes from the new stream. The engine
   ///   reaches this through <c>ApplySessionSeed</c>, so a member configured with a Faker source follows the
   ///   session seed.
   /// </summary>
   public override void SetSeedToRandomValue(int seed) {
      base.SetSeedToRandomValue(seed);
      _seed = seed;
      _faker = null;
   }

   /// <summary>
   ///   Takes the next value from the faker.
   /// </summary>
   protected abstract T GetFakerValue(BogusFaker faker);

   protected override T GetNextValue(IGenerationContext? context) {
      if (_decidesAboutNullItself && NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return default!;
      }

      return GetFakerValue(Faker);
   }

   private BogusFaker CreateFaker(int seed)
      => new(_locale) { Random = new StableRandomizer(seed) };
}
