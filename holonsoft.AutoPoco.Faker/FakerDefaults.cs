namespace holonsoft.AutoPoco.Faker;

/// <summary>
///   Library defaults of the Faker integration. Read-only on purpose, the same way
///   <see cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults" /> is: a source takes what it needs
///   through its constructor.
/// </summary>
public static class FakerDefaults {
   /// <summary>
   ///   The locale every source uses when none is given. Bogus writes locales lower case, with an underscore
   ///   before the region, e.g. <c>en</c>, <c>en_US</c>, <c>de</c>, <c>de_AT</c>, <c>de_CH</c>.
   /// </summary>
   public const string Locale = "en";
}

/// <summary>
///   The bounds a price source uses when none are given.
/// </summary>
public static class FakerPriceDefaults {
   /// <summary>Lower bound, one unit of the currency.</summary>
   public const decimal Min = 1m;

   /// <summary>Upper bound.</summary>
   public const decimal Max = 1000m;

   /// <summary>Decimals of a normal money amount.</summary>
   public const int Decimals = 2;
}

/// <summary>
///   The bounds a date source uses when none are given.
/// </summary>
/// <remarks>
///   Fixed dates on purpose. Bogus also offers <c>Date.Past</c> and <c>Date.Future</c>, which measure from
///   the current time, so the same seed would produce a different value tomorrow. A source of this library
///   never reads the clock.
/// </remarks>
public static class FakerDateDefaults {
   /// <summary>Lower bound, the first of January 2000.</summary>
   public static readonly DateTime Min = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

   /// <summary>Upper bound, the last day of 2035.</summary>
   public static readonly DateTime Max = new(2035, 12, 31, 23, 59, 59, DateTimeKind.Unspecified);
}
