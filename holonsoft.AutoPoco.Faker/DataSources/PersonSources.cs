using holonsoft.AutoPoco.Configuration;
using BogusFaker = Bogus.Faker;

namespace holonsoft.AutoPoco.Faker.DataSources;

/// <summary>
///   A first name from the locale of the source, e.g. "Marie" for <c>de</c> and "Mary" for <c>en</c>.
/// </summary>
public abstract class FakerFirstNameSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Name.FirstName();
}

/// <inheritdoc cref="FakerFirstNameSourceBase" />
public class FakerFirstNameSource(string? locale) : FakerFirstNameSourceBase(locale) {
   public FakerFirstNameSource() : this(null) { }
}

/// <summary>
///   A first name that returns null every now and then.
/// </summary>
public class NullableFakerFirstNameSource(string? locale, int nullCreationThreshold)
   : FakerFirstNameSourceBase(locale, nullCreationThreshold) {
   public NullableFakerFirstNameSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerFirstNameSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerFirstNameSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A last name from the locale of the source.
/// </summary>
public abstract class FakerLastNameSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Name.LastName();
}

/// <inheritdoc cref="FakerLastNameSourceBase" />
public class FakerLastNameSource(string? locale) : FakerLastNameSourceBase(locale) {
   public FakerLastNameSource() : this(null) { }
}

/// <summary>
///   A last name that returns null every now and then.
/// </summary>
public class NullableFakerLastNameSource(string? locale, int nullCreationThreshold)
   : FakerLastNameSourceBase(locale, nullCreationThreshold) {
   public NullableFakerLastNameSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerLastNameSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerLastNameSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}

/// <summary>
///   A full name from the locale of the source, put together the way the locale writes it, sometimes with a
///   prefix or a suffix.
/// </summary>
/// <remarks>
///   The first and the last name of a full name do not come from <see cref="FakerFirstNameSourceBase" /> or
///   <see cref="FakerLastNameSourceBase" />. Configure the members you need separately when the parts of a
///   name have to match each other.
/// </remarks>
public abstract class FakerFullNameSourceBase(string? locale, int? nullCreationThreshold = null)
   : FakerSourceBase<string>(locale, nullCreationThreshold) {
   protected override string GetFakerValue(BogusFaker faker)
      => faker.Name.FullName();
}

/// <inheritdoc cref="FakerFullNameSourceBase" />
public class FakerFullNameSource(string? locale) : FakerFullNameSourceBase(locale) {
   public FakerFullNameSource() : this(null) { }
}

/// <summary>
///   A full name that returns null every now and then.
/// </summary>
public class NullableFakerFullNameSource(string? locale, int nullCreationThreshold)
   : FakerFullNameSourceBase(locale, nullCreationThreshold) {
   public NullableFakerFullNameSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerFullNameSource(string? locale) : this(locale, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableFakerFullNameSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
