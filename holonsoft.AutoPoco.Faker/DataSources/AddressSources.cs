using holonsoft.AutoPoco.Configuration;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.DataSources;

/// <summary>
///   A street with a house number, written the way the locale writes it.
/// </summary>
/// <remarks>
///   Street, city, postal code and country are separate sources and are drawn independently, so they do not
///   describe one real place. Configure a lambda source when the parts of an address have to match.
/// </remarks>
public abstract class FakerStreetAddressSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Address.StreetAddress();
}

/// <inheritdoc cref="FakerStreetAddressSourceBase" />
public class FakerStreetAddressSource(string? locale) : FakerStreetAddressSourceBase(locale) {
   public FakerStreetAddressSource() : this(null) { }
}

/// <summary>
///   A street with a house number that returns null every now and then.
/// </summary>
public class NullableFakerStreetAddressSource(string? locale, int nullCreationThreshold)
   : FakerStreetAddressSourceBase(locale, nullCreationThreshold) {
   public NullableFakerStreetAddressSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerStreetAddressSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerStreetAddressSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A city name from the locale of the source.
/// </summary>
public abstract class FakerCitySourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Address.City();
}

/// <inheritdoc cref="FakerCitySourceBase" />
public class FakerCitySource(string? locale) : FakerCitySourceBase(locale) {
   public FakerCitySource() : this(null) { }
}

/// <summary>
///   A city name that returns null every now and then.
/// </summary>
public class NullableFakerCitySource(string? locale, int nullCreationThreshold)
   : FakerCitySourceBase(locale, nullCreationThreshold) {
   public NullableFakerCitySource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCitySource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCitySource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A country name, written in the language of the locale.
/// </summary>
public abstract class FakerCountrySourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Address.Country();
}

/// <inheritdoc cref="FakerCountrySourceBase" />
public class FakerCountrySource(string? locale) : FakerCountrySourceBase(locale) {
   public FakerCountrySource() : this(null) { }
}

/// <summary>
///   A country name that returns null every now and then.
/// </summary>
public class NullableFakerCountrySource(string? locale, int nullCreationThreshold)
   : FakerCountrySourceBase(locale, nullCreationThreshold) {
   public NullableFakerCountrySource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCountrySource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerCountrySource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A postal code in the format the locale uses. The code is well formed, it does not belong to a city a
///   sibling member produced.
/// </summary>
public abstract class FakerPostalCodeSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Address.ZipCode();
}

/// <inheritdoc cref="FakerPostalCodeSourceBase" />
public class FakerPostalCodeSource(string? locale) : FakerPostalCodeSourceBase(locale) {
   public FakerPostalCodeSource() : this(null) { }
}

/// <summary>
///   A postal code that returns null every now and then.
/// </summary>
public class NullableFakerPostalCodeSource(string? locale, int nullCreationThreshold)
   : FakerPostalCodeSourceBase(locale, nullCreationThreshold) {
   public NullableFakerPostalCodeSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPostalCodeSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPostalCodeSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
