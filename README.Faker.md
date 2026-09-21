# holonsoft.AutoPoco.Faker

Realistic values for [AutoPoco](https://github.com/holonsoft/AutoPoco), taken from [Bogus](https://github.com/bchavez/Bogus).

```
dotnet add package holonsoft.AutoPoco.Faker
```

The split of work is the whole idea. Bogus is good at a single believable value, AutoPoco is good at everything around it: building the object graph, keeping members apart, following relationships and repeating the whole thing for a seed. Bogus never builds an object here, it only answers "give me a city name".

## The sources

| Source | Produces |
| --- | --- |
| `FakerFirstNameSource`, `FakerLastNameSource`, `FakerFullNameSource` | Names in the language of the locale |
| `FakerEmailAddressSource` | An email address on one of the example domains |
| `FakerPhoneNumberSource` | A phone number in the format of the locale |
| `FakerStreetAddressSource`, `FakerCitySource`, `FakerPostalCodeSource`, `FakerCountrySource` | The parts of an address |
| `FakerCompanyNameSource` | A company name including its legal form, e.g. "Müller GmbH" |
| `FakerProductNameSource` | A product name that reads like a catalogue entry |
| `FakerPriceSource` | A `decimal` amount between two bounds, 1 to 1000 with 2 decimals by default |
| `FakerCurrencyCodeSource` | An ISO 4217 code such as `EUR` |
| `FakerDateTimeSource` | A point in time between two bounds, 2000-01-01 to 2035-12-31 by default |

Every one of them has a `Nullable...` variant, like the sources of the core package.

```csharp
var factory = AutoPocoContainer.Configure(x => {
   x.UseSeed(4711);

   x.Include<Customer>()
     .Setup(c => c.FirstName).Use<FakerFirstNameSource>("de")
     .Setup(c => c.LastName).Use<FakerLastNameSource>("de")
     .Setup(c => c.EmailAddress).Use<FakerEmailAddressSource>("de")
     .Setup(c => c.City).Use<FakerCitySource>("de");
});

// the same customers, every run, in every process
var customers = factory.CreateSession().Collection<Customer>(1000);
```

The first argument is the Bogus locale, written lower case with an underscore before the region: `en`, `en_US`, `de`, `de_AT`, `de_CH` and so on. Leave it out for the default `en`. Every locale of the pinned Bogus version works with every source, and where a locale misses a piece of data Bogus falls back to English rather than failing, so an unusual locale can give you an English city.

## Seeds and the limits of the guarantee

The AutoPoco session seed drives Bogus. Every source instance owns one Bogus faker, that faker draws from AutoPoco's own `StableRandom`, and the static `Bogus.Randomizer.Seed` is never read or written. So the same seed gives the same data and each member keeps its own stream.

One limit worth knowing. AutoPoco guarantees a sequence across .NET versions because it brings its own generator. Bogus takes its values from locale data that ships inside its package, so a different Bogus version can give different names for the same seed. This package therefore depends on **exactly** Bogus 35.6.5, not "35.6.5 or newer". If your project already references a different Bogus version you get a NuGet version conflict instead of a silent change of your test data, which is the trade this package makes on purpose. Pin this package too when your tests compare against stored data.

## What stays in the core package

Numbers with a check digit. Bogus has a `Commerce.Ean` of its own whose check digit is not guaranteed to be valid, so use `Ean13Source`, `GtinSource`, `IsbnSource` and `AsinSource` from `holonsoft.AutoPoco` for article numbers and let Bogus do the names.

The sources are also independent of each other. A `FakerCitySource` and a `FakerPostalCodeSource` on the same object give a correct city and a correct postal code, but not a postal code that belongs to that city, and an email address does not contain the name of the customer it sits on. Build the member from the finished object where the parts have to match:

```csharp
var customers = factory.CreateSession().Collection<Customer>(1000, c =>
   c.Impose(x => x.EmailAddress, (_, x) => $"{x.FirstName}.{x.LastName}@example.com".ToLowerInvariant()));
```

## The two packages together

```csharp
x.Include<Product>()
  .Setup(p => p.Name).Use<FakerProductNameSource>("de")     // Bogus, believable
  .Setup(p => p.Ean).Use<Ean13Source>("40063")              // core, check digit is correct
  .Setup(p => p.Price).Use<FakerPriceSource>(5m, 500m, 2, "de");

x.Include<Order>()
  .Setup(o => o.Customer).Use<AutoSource<Customer>>()       // AutoPoco, the graph
  .Setup(o => o.Lines).Collection(1, 4);
```

Bogus creates believable single values, AutoPoco creates the consistent, repeatable object graph around them. That combination is what fills a shared developer database.

Full documentation of AutoPoco itself: https://github.com/holonsoft/AutoPoco
