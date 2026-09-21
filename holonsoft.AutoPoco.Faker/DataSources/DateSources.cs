using holonsoft.AutoPoco.Configuration;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.DataSources;

/// <summary>
///   A point in time between two bounds.
/// </summary>
/// <typeparam name="T">
///   <see cref="DateTime" /> or <see cref="Nullable{T}" /> of it.
/// </typeparam>
public abstract class FakerDateTimeSourceBase<T> : FakerSourceBase<T> {
   private readonly DateTime _min;
   private readonly DateTime _max;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="min">Lower bound.</param>
   /// <param name="max">Upper bound.</param>
   /// <param name="locale">A Bogus locale, <see cref="FakerDefaults.Locale" /> when null.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="max" /> lies before <paramref name="min" />.</exception>
   protected FakerDateTimeSourceBase(DateTime min, DateTime max, string? locale, int? nullCreationThreshold = null)
      : base(locale, nullCreationThreshold) {
      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum date must not lie before the minimum date ({min:O}).");

      // the value is handed over as a DateTime, a T that cannot hold one would only fail on the first draw
      if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?))
         throw new InvalidOperationException($"A point in time is a DateTime, '{typeof(T).Name}' cannot hold one.");

      _min = min;
      _max = max;
   }

   /// <summary>
   ///   Lower bound.
   /// </summary>
   public DateTime Min => _min;

   /// <summary>
   ///   Upper bound.
   /// </summary>
   public DateTime Max => _max;

   protected override T GetFakerValue(BogusFaker faker)
      => (T) (object) faker.Date.Between(_min, _max);
}

/// <inheritdoc cref="FakerDateTimeSourceBase{T}" />
public class FakerDateTimeSource(DateTime min, DateTime max, string? locale)
   : FakerDateTimeSourceBase<DateTime>(min, max, locale) {
   public FakerDateTimeSource() : this(FakerDateDefaults.Min, FakerDateDefaults.Max, null) { }

   public FakerDateTimeSource(DateTime min, DateTime max) : this(min, max, null) { }
}

/// <summary>
///   A point in time between two bounds that returns null every now and then.
/// </summary>
public class NullableFakerDateTimeSource(DateTime min, DateTime max, string? locale, int nullCreationThreshold)
   : FakerDateTimeSourceBase<DateTime?>(min, max, locale, nullCreationThreshold) {
   public NullableFakerDateTimeSource()
      : this(FakerDateDefaults.Min, FakerDateDefaults.Max, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerDateTimeSource(int nullCreationThreshold)
      : this(FakerDateDefaults.Min, FakerDateDefaults.Max, null, nullCreationThreshold) { }

   public NullableFakerDateTimeSource(DateTime min, DateTime max)
      : this(min, max, null, AutoPocoDefaults.NullCreationThreshold) { }
}
