using holonsoft.AutoPoco.Configuration;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.DataSources;

/// <summary>
///   A company name, built the way the locale builds one, e.g. "Müller GmbH" for <c>de</c>.
/// </summary>
public abstract class FakerCompanyNameSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Company.CompanyName();
}

/// <inheritdoc cref="FakerCompanyNameSourceBase" />
public class FakerCompanyNameSource(string? locale) : FakerCompanyNameSourceBase(locale) {
   public FakerCompanyNameSource() : this(null) { }
}

/// <summary>
///   A company name that returns null every now and then.
/// </summary>
public class NullableFakerCompanyNameSource(string? locale, int nullCreationThreshold)
   : FakerCompanyNameSourceBase(locale, nullCreationThreshold) {
   public NullableFakerCompanyNameSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCompanyNameSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCompanyNameSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A product name, e.g. "Handcrafted Wooden Chair".
/// </summary>
/// <remarks>
///   Bogus builds the name from an adjective, a material and a product, so the result reads like a catalogue
///   entry without describing a real article. For the article number use the identifier sources of the core
///   package, <c>Ean13Source</c> and friends, whose check digit is correct. Bogus has a
///   <c>Commerce.Ean</c> of its own, its check digit is not guaranteed to be valid.
/// </remarks>
public abstract class FakerProductNameSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Commerce.ProductName();
}

/// <inheritdoc cref="FakerProductNameSourceBase" />
public class FakerProductNameSource(string? locale) : FakerProductNameSourceBase(locale) {
   public FakerProductNameSource() : this(null) { }
}

/// <summary>
///   A product name that returns null every now and then.
/// </summary>
public class NullableFakerProductNameSource(string? locale, int nullCreationThreshold)
   : FakerProductNameSourceBase(locale, nullCreationThreshold) {
   public NullableFakerProductNameSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerProductNameSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerProductNameSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   The three letter ISO 4217 code of a currency, e.g. "EUR".
/// </summary>
public abstract class FakerCurrencyCodeSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Finance.Currency().Code;
}

/// <inheritdoc cref="FakerCurrencyCodeSourceBase" />
public class FakerCurrencyCodeSource(string? locale) : FakerCurrencyCodeSourceBase(locale) {
   public FakerCurrencyCodeSource() : this(null) { }
}

/// <summary>
///   An ISO 4217 currency code that returns null every now and then.
/// </summary>
public class NullableFakerCurrencyCodeSource(string? locale, int nullCreationThreshold)
   : FakerCurrencyCodeSourceBase(locale, nullCreationThreshold) {
   public NullableFakerCurrencyCodeSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCurrencyCodeSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCurrencyCodeSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   An amount of money between two bounds, rounded to a number of decimals. Meant for a price, a net value
///   or any other money member.
/// </summary>
/// <typeparam name="T">
///   <see cref="decimal" /> or <see cref="Nullable{T}" /> of it.
/// </typeparam>
public abstract class FakerPriceSourceBase<T> : FakerSourceBase<T> {
   private readonly decimal _min;
   private readonly decimal _max;
   private readonly int _decimals;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="min">Lower bound, inclusive.</param>
   /// <param name="max">Upper bound.</param>
   /// <param name="decimals">How many decimals the amount is rounded to, 2 for a normal price.</param>
   /// <param name="locale">A Bogus locale, <see cref="FakerDefaults.Locale" /> when null.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="max" /> lies below <paramref name="min" />, or <paramref name="decimals" /> is
   ///   negative or beyond the precision of a decimal.
   /// </exception>
   protected FakerPriceSourceBase(decimal min, decimal max, int decimals, string? locale, int? nullCreationThreshold = null)
      : base(locale, nullCreationThreshold) {
      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum amount must not be below the minimum amount ({min}).");

      if (decimals is < 0 or > 28)
         throw new ArgumentOutOfRangeException(nameof(decimals), decimals, "An amount is rounded to between 0 and 28 decimals.");

      // the value is handed over as a decimal, a T that cannot hold one would only fail on the first draw
      if (typeof(T) != typeof(decimal) && typeof(T) != typeof(decimal?))
         throw new InvalidOperationException($"An amount is a decimal, '{typeof(T).Name}' cannot hold one.");

      _min = min;
      _max = max;
      _decimals = decimals;
   }

   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public decimal Min => _min;

   /// <summary>
   ///   Upper bound.
   /// </summary>
   public decimal Max => _max;

   /// <summary>
   ///   How many decimals the amount is rounded to.
   /// </summary>
   public int Decimals => _decimals;

   protected override T GetFakerValue(BogusFaker faker)
      => (T) (object) faker.Finance.Amount(_min, _max, _decimals);
}

/// <inheritdoc cref="FakerPriceSourceBase{T}" />
public class FakerPriceSource(decimal min, decimal max, int decimals, string? locale)
   : FakerPriceSourceBase<decimal>(min, max, decimals, locale) {
   public FakerPriceSource() : this(FakerPriceDefaults.Min, FakerPriceDefaults.Max, FakerPriceDefaults.Decimals, null) { }

   public FakerPriceSource(decimal min, decimal max) : this(min, max, FakerPriceDefaults.Decimals, null) { }

   public FakerPriceSource(decimal min, decimal max, int decimals) : this(min, max, decimals, null) { }
}

/// <summary>
///   An amount of money between two bounds that returns null every now and then.
/// </summary>
public class NullableFakerPriceSource(decimal min, decimal max, int decimals, string? locale, int nullCreationThreshold)
   : FakerPriceSourceBase<decimal?>(min, max, decimals, locale, nullCreationThreshold) {
   public NullableFakerPriceSource()
      : this(FakerPriceDefaults.Min, FakerPriceDefaults.Max, FakerPriceDefaults.Decimals, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPriceSource(int nullCreationThreshold)
      : this(FakerPriceDefaults.Min, FakerPriceDefaults.Max, FakerPriceDefaults.Decimals, null, nullCreationThreshold) { }

   public NullableFakerPriceSource(decimal min, decimal max)
      : this(min, max, FakerPriceDefaults.Decimals, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPriceSource(decimal min, decimal max, int decimals)
      : this(min, max, decimals, null, AutoPocoDefaults.NullCreationThreshold) { }
}
