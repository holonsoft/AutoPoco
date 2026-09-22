using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   Generates a lot or batch number the way manufacturing plants stamp them: an L, a date code of two
///   digit year and three digit day of the year, a hyphen and a drawn suffix, e.g. <c>L26265-0387</c> for
///   the 265th day of 2026, batch 0387.
/// </summary>
/// <remarks>
///   Lot numbers follow no standard, but the year plus day-of-year date code is the widespread convention,
///   so the default looks like the real thing. The date is drawn between the two bounds from the seeded
///   stream, never from the clock, so the same seed gives the same lots on every run. The suffix comes from
///   a pattern, see <see cref="PatternSourceBase" /> for the placeholders; the date code itself is not part
///   of the pattern and always present. Where your plant writes lots differently, use
///   <see cref="PatternSource" /> directly.
/// </remarks>
public abstract class LotNumberSourceBase : PatternSourceBase {
   private static readonly DateOnly _defaultMinDate = new(2020, 1, 1);
   private static readonly DateOnly _defaultMaxDate = new(2035, 12, 31);

   /// <summary>
   ///   The suffix pattern used when none is given: four digits.
   /// </summary>
   public const string DefaultSuffixPattern = "####";

   private readonly DateOnly _minDate;
   private readonly DateOnly _maxDate;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="minDate">Lower bound of the date code, inclusive; the first of January 2020 when null.</param>
   /// <param name="maxDate">Upper bound of the date code, inclusive; the last day of 2035 when null.</param>
   /// <param name="suffixPattern">The pattern behind the hyphen, four digits when null.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxDate" /> lies before <paramref name="minDate" />.</exception>
   /// <exception cref="ArgumentException"><paramref name="suffixPattern" /> is empty or ends in a lone backslash.</exception>
   protected LotNumberSourceBase(DateOnly? minDate = null, DateOnly? maxDate = null, string? suffixPattern = null, int? nullCreationThreshold = null)
      : base(suffixPattern ?? DefaultSuffixPattern, nullCreationThreshold) {
      _minDate = minDate ?? _defaultMinDate;
      _maxDate = maxDate ?? _defaultMaxDate;

      if (_maxDate < _minDate)
         throw new ArgumentOutOfRangeException(nameof(maxDate), _maxDate, $"The maximum date must not be before the minimum date ({_minDate:O}).");
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var days = _maxDate.DayNumber - _minDate.DayNumber;
      var date = DateOnly.FromDayNumber(_minDate.DayNumber + Random.Next(0, days + 1));

      return $"L{date.Year % 100:00}{date.DayOfYear:000}-{FillPattern(Pattern)}";
   }
}

/// <summary>
///   A lot number with a seeded date code, e.g. <c>L26265-0387</c>.
/// </summary>
public class LotNumberSource(DateOnly? minDate, DateOnly? maxDate, string? suffixPattern) : LotNumberSourceBase(minDate, maxDate, suffixPattern) {
   public LotNumberSource() : this(null, null, null) { }

   public LotNumberSource(string suffixPattern) : this(null, null, suffixPattern) { }

   public LotNumberSource(DateOnly minDate, DateOnly maxDate) : this(minDate, maxDate, null) { }
}

/// <summary>
///   A lot number that returns null every now and then.
/// </summary>
public class NullableLotNumberSource(DateOnly? minDate, DateOnly? maxDate, string? suffixPattern, int nullCreationThreshold)
   : LotNumberSourceBase(minDate, maxDate, suffixPattern, nullCreationThreshold) {
   public NullableLotNumberSource() : this(null, null, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableLotNumberSource(string suffixPattern) : this(null, null, suffixPattern, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableLotNumberSource(DateOnly minDate, DateOnly maxDate) : this(minDate, maxDate, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableLotNumberSource(int nullCreationThreshold) : this(null, null, null, nullCreationThreshold) { }
}
