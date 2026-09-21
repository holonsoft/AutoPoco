using holonsoft.AutoPoco.Configuration;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.DataSources;

/// <summary>
///   An email address built from a name of the locale and one of the example domains Bogus ships.
/// </summary>
/// <remarks>
///   The local part comes from a name Bogus draws itself, it has nothing to do with a first or last name
///   member of the same object. Build the address from a lambda source when it has to match the person.
/// </remarks>
public abstract class FakerEmailAddressSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Internet.Email();
}

/// <inheritdoc cref="FakerEmailAddressSourceBase" />
public class FakerEmailAddressSource(string? locale) : FakerEmailAddressSourceBase(locale) {
   public FakerEmailAddressSource() : this(null) { }
}

/// <summary>
///   An email address that returns null every now and then.
/// </summary>
public class NullableFakerEmailAddressSource(string? locale, int nullCreationThreshold)
   : FakerEmailAddressSourceBase(locale, nullCreationThreshold) {
   public NullableFakerEmailAddressSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerEmailAddressSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerEmailAddressSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A phone number in the format the locale uses.
/// </summary>
/// <remarks>
///   The number follows a format of the locale, it is not checked against a real numbering plan, so the area
///   code may not exist. Do not feed it to a dialler.
/// </remarks>
public abstract class FakerPhoneNumberSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Phone.PhoneNumber();
}

/// <inheritdoc cref="FakerPhoneNumberSourceBase" />
public class FakerPhoneNumberSource(string? locale) : FakerPhoneNumberSourceBase(locale) {
   public FakerPhoneNumberSource() : this(null) { }
}

/// <summary>
///   A phone number that returns null every now and then.
/// </summary>
public class NullableFakerPhoneNumberSource(string? locale, int nullCreationThreshold)
   : FakerPhoneNumberSourceBase(locale, nullCreationThreshold) {
   public NullableFakerPhoneNumberSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPhoneNumberSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerPhoneNumberSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
