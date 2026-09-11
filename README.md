# holonsoft / AutoPoco

[![CI](https://github.com/holonsoft/AutoPoco/actions/workflows/ci.yml/badge.svg)](https://github.com/holonsoft/AutoPoco/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/holonsoft.AutoPoco.svg)](https://www.nuget.org/packages/holonsoft.AutoPoco/)

AutoPoco is a highly configurable framework for the purpose of fluently building readable (test) data.
holonsoft ported this famous lib to newest version of dotnet

# New in 6.0.0 (in progress)

## Breaking changes at a glance
* Every generated sequence changed once, mainly because every member has its own random stream now. Tests that pin generated values need new expectations. This is the last time, see "Why the sequences changed" below.
* `IntegerSource`, `NullableIntegerSource`, `LongSource`, `NullableLongSource`, `RandomNumberSource` and `NullableRandomNumberSource` treat `max` as **inclusive** now, it used to be exclusive. `new IntegerSource(1, 3)` produces 3 as well. This matches `NumberSource<T>` and the date sources: the maximum of every integer and date source is inclusive now.
* `Int128Source` and `NullableInt128Source` pick uniformly from their range instead of generating a value and clamping it to the bounds. The old implementation never produced a negative value and returned the upper bound almost always on a restricted range.
* `AutoPocoGlobalSettings` is gone, the defaults are read-only constants in `AutoPocoDefaults`.
* The never implemented `Ctor(...)` stub on the type builder is gone.
* The lambdas receiving the generation context get a non-nullable `IGenerationContext`, no more `ctx!`.
* Recompile needed, source code unchanged: four engine constructors gained optional parameters, `LongSourceBase.SetMinMax` takes `long` instead of `int` (it could not reach the range of its own source before) and `Int128IdSource.SetStartValue` takes `Int128` instead of `long`.

## Features and fixes
* Lambda data sources: compute a member value inline instead of writing a data source class

```CSHARP
var counter = 0;
var factory = AutoPocoContainer.Configure(x => {
   x.Conventions(c => c.UseDefaultConventions());
   x.Include<SimpleUser>()
      // runs once per generated object
      .Setup(c => c.Id).From(() => ++counter)
      .Setup(c => c.EmailAddress).From(() => $"user{counter}@example.org")
      // the context variant can build related objects
      .Setup(c => c.Role).From(ctx => ctx.Single<SimpleUserRole>().Impose(r => r.Name, "Guest").Get());
});

// the same at generation time, overriding the configuration for this call only
var users = session.List<SimpleUser>(10)
   .Source(u => u.LastName, () => "Smith")
   .Get();
```

  The lambda is in charge of the value, including null. `FuncSource<T>` is the class behind it and can be used directly wherever an `IDataSource` is expected.
* Nullability-aware null generation, opt-in: `x.RespectNullableAnnotations(threshold)` turns every property or field declared as nullable (`string?`, `int?`, `DateTime?`, ...) into null with the given probability in percent (default 15), whatever data source is configured for it. Members without a nullable annotation, members of types compiled without nullable annotations and values set by `Impose` are never touched.

```CSHARP
var factory = AutoPocoContainer.Configure(x => {
   x.RespectNullableAnnotations(20);   // 20 percent of the nullable members become null
   x.Conventions(c => c.UseDefaultConventions());
   x.Include<Customer>()
      .Setup(c => c.City).Use<CitySource>();   // City is string?, so it is null now and then
});
```

  Every nullable member gets its own deterministic null pattern, so the nullable members of one object do not all become null at once, and two sessions of the same factory produce the same pattern. Sources that produce nulls on their own (the `Nullable*` sources) keep doing so on top of it. Generation time overrides via `Source(...)` are covered as well.
* `NumberSource<T>` and `NullableNumberSource<T>`: one generic source for every built-in numeric type (`byte`, `short`, `int`, `long`, `Int128`, their unsigned variants, `nint`, `float`, `double`, `Half`, `decimal`, even `char`), with `min` and `max`. Integer types are drawn uniformly from the inclusive range, both bounds can be produced, the whole range of the type is the default. Floating point types and `decimal` are interpolated between the bounds without overflowing, even for the whole range of the type. A maximum below the minimum, `NaN` or an infinite bound throws an `ArgumentOutOfRangeException`. The existing `IntegerSource`, `DecimalSource` and friends stay as they are.

```CSHARP
x.Include<Order>()
   .Setup(c => c.Rating).Use<NumberSource<byte>>((byte) 1, (byte) 5)
   .Setup(c => c.Amount).Use<NumberSource<decimal>>(1m, 100m)
   .Setup(c => c.ExternalId).Use<NumberSource<UInt128>>(s => s.SetMin(1UL))
   .Setup(c => c.Discount).Use<NullableNumberSource<decimal>>(0m, 30m, 50);   // null in 50 percent of the cases
```

* Records and immutable types are constructed through their constructor, no parameterless constructor needed anymore. Constructor parameters are fed from the registered members with the same name (case-insensitive) and a compatible type, so `Setup(c => c.FirstName).Use<FirstNameSource>()` on a positional record property ends up as the `FirstName` argument, and a get-only property of a class ends up as the matching argument. Members taken by the constructor are not set again afterwards. `Impose` and `Source(...)` at generation time keep working on positional record properties.

```CSHARP
public record Customer(string FirstName, string LastName, string? Nickname, DateOnly Birthday, Address Address);

public class Money {
   public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }
   public decimal Amount { get; }
   public string Currency { get; }
}

var factory = AutoPocoContainer.Configure(x => {
   x.Conventions(c => c.UseDefaultConventions());
   x.Include<Customer>()
      .Setup(c => c.FirstName).Use<FirstNameSource>()
      .Setup(c => c.LastName).Use<LastNameSource>()
      .Setup(c => c.Birthday).Use<DateOnlySource>(new DateOnly(1950, 1, 1), new DateOnly(2000, 12, 31));
   x.Include<Money>()
      .Setup(c => c.Amount).Use<NumberSource<decimal>>(1m, 100m)
      .Setup(c => c.Currency).From(() => "CHF");
});
```

  The rules: the constructor that takes the most members which cannot be set otherwise wins, then the one with the fewest parameters, so ordinary classes keep using their parameterless constructor and their setters. With the default conventions get-only properties that match a constructor parameter are registered too, so an unconfigured `Money` gets the default member values through its constructor. A parameter without a matching member gets its declared default value, the default of its value type, or for other reference types an object generated by the session. A struct without any configured member stays at its default, so an unconfigured `DateOnly` parameter no longer throws. A get-only property with a data source but no matching constructor parameter throws an `InvalidOperationException` that names the property. `CtorSource<T>` is the class behind it; `new CtorSource<T>(constructorInfo)` pins a constructor and still takes the matching members. Constructor arguments honor `RespectNullableAnnotations` and the recursion limit, so a self referencing record such as `record Node(string Name, Node? Child)` terminates.
* Seeds: `x.UseSeed(seed)` sets the seed of a factory, `factory.CreateSession(recursionLimit, seed)` gives a single session another seed. Every data source in a session gets its own random stream derived from the session seed and the member it feeds (`Type.Member`), so two members with the same source type no longer produce the same values, the nullable sources of one object no longer become null together, and the values of a member do not depend on which other members are configured. A seed set on a source in the configuration (`Use<IntegerSource>(s => s.SetSeedToRandomValue(7))`) wins over the session seed. The null patterns of `RespectNullableAnnotations`, the shuffle behind `Random(count)` and constructor arguments follow the seed as well.

```CSHARP
var factory = AutoPocoContainer.Configure(x => {
   x.UseSeed(2024);   // default is AutoPocoDefaults.Seed (1337)
   x.Include<SimpleUser>()
      .Setup(c => c.FirstName).Use<FirstNameSource>()
      .Setup(c => c.LastName).Use<LastNameSource>();
});

var session = factory.CreateSession();          // seed 2024
var other = factory.CreateSession(5, 4711);      // seed 4711, same configuration
```

* Own random number generator: every source draws from `StableRandom` (xoshiro256** seeded through SplitMix64), a `Random` subclass owned by AutoPoco, instead of `System.Random`. The sequence for a seed is defined by AutoPoco's code alone, so test data stays the same across .NET versions; Microsoft explicitly does not promise that for a seeded `System.Random`. The generator is pinned by tests. Rule from now on: catalogs (names, cities, zip codes, ...) and source algorithms are frozen within a major version, a change to either is a major version bump.

* Breaking: `AutoPocoGlobalSettings` is gone, it was mutable process-wide state that leaked between test fixtures. The defaults are read-only constants in `AutoPocoDefaults` (`Seed`, `NullCreationThreshold`, `RecursionLimit`). Set a seed with `UseSeed`, null thresholds per source or with `RespectNullableAnnotations(threshold)`.
* Breaking: every generated sequence changed once with 6.0, mainly because every member has its own random stream now and to a smaller part because of the own generator. Tests that pin generated values need new expectations, this is the last time. "Why the sequences changed" at the end of this section has the full story.
* Index-aware `Impose`: on a list or a selection the imposed value can depend on the position of the item (0 based), and on the item as generated so far. The position is the one in the whole list, also inside `First`/`Next` and after `Random`. A single generator gets `Impose(member, item => value)` for values that depend on other members.

```CSHARP
var users = session.List<SimpleUser>(100)
   .Impose(u => u.Id, i => i + 1)                                            // 1..100
   .Impose(u => u.EmailAddress, (i, u) => $"{u.FirstName}.{u.LastName}{i}@example.test")
   .First(10).Impose(u => u.City, i => $"Branch {i}")
   .All()
   .Get();

var admin = session.Single<SimpleUser>()
   .Impose(u => u.LastName, "Ashton")
   .Impose(u => u.EmailAddress, u => $"{u.FirstName}.{u.LastName}@example.test")
   .Get();
```

  The item lambda runs after the configured sources and after every `Impose` registered before it, so it sees those values.
* `Invoke(c => c.Method(...))` in the configuration takes lambdas and values as arguments now: `Use.From(() => value)` and `Use.From(ctx => ...)` compute an argument per object, constants and captured variables arrive as their value (constants used to arrive as the expression object, a bug), `null` is allowed. Any other method call in the argument list is rejected with a message that names the options.

```CSHARP
var counter = 0;
x.Include<SimpleUser>()
   .Invoke(c => c.SetPassword(Use.From(() => $"pw-{++counter}")))
   .Invoke(c => c.SetSomething("fixed", Use.Source<string, LastNameSource>()!));
```

* The lambdas receiving the generation context (`From(ctx => ...)`, `Source(member, ctx => ...)`, `Use.From(ctx => ...)`) get a non-nullable `IGenerationContext`, no more `ctx!`. A `FuncSource` built from such a lambda throws a clear `InvalidOperationException` when used outside of a session.
* Hardening: the public configuration and generation API validates its arguments (`ArgumentNullException`, `ArgumentOutOfRangeException` for negative counts, `ArgumentException` for unknown members, non-source types and non-convention types) and every internal failure carries the type and member it happened at. The never implemented `Ctor(...)` stub on the type builder is gone. The package ships an XML documentation file.
* `IDataSource<T>` is covariant now, so a source of `string` (e.g. `CitySource`) can be used for a `string?` member without a nullability warning.
* Bug fix: the integer sources that existed before `NumberSource<T>` never produced their maximum, `new IntegerSource(1, 3)` gave 1 or 2 only. `IntegerSource`, `LongSource`, `RandomNumberSource` and their nullable variants draw uniformly from the inclusive range now, both bounds can be produced. `Int128Source` and `NullableInt128Source` were worse: they built a value from two non-negative draws and clamped it to the range, so the default source never produced a negative value and `new Int128Source(1, 5)` returned 5 almost every time. All of them share the pick of `NumberSource<T>` now. `LongSourceBase.SetMinMax` takes `long` arguments at last, and all four throw an `ArgumentOutOfRangeException` when the maximum lies below the minimum. The generated sequences of the long and Int128 sources changed, the ones of the integer sources did not.
* Bug fix: every `Nullable*` source ignored an explicit null creation threshold. The fixed array and dictionary based sources (names, companies, cities, capitals, countries, states, zip codes, urls) even used the threshold as their random seed. The threshold is honored now. As a consequence the null positions in the stable sequences of these sources changed, the data itself comes in the same order as before.
* Bug fix: `DateTimeSource`, `DateOnlySource`, `TimeOnlySource` and `DateOfBirthSource` never produced the upper end of their ranges: no December, no 31st, no 23:00, no minute or second 59, and the maximum year of a date of birth was never reached. Ranges within a single year could produce values outside the range. All four sources now pick uniformly from the whole range, both bounds inclusive, and throw an `ArgumentOutOfRangeException` when the maximum lies before the minimum. `TimeOnlySource` supports ranges that wrap around midnight (22:00 to 02:00). `DateTimeSource` keeps the `DateTimeKind` of the minimum date. The generated sequences for a given seed changed.
* Bug fix: `RandomUtfTextSource` could loop forever when it hit a Unicode block without any allowed character.
* Build and packaging: GitHub Actions CI, trusted publishing to nuget.org, MinVer versioning from git tags, central package management, tests on xunit.v3 for net8/9/10
* The library no longer drags FluentAssertions and Moq into your project as dependencies

## Why the sequences changed
A fair question when you upgrade and every pinned test value is suddenly wrong: was something broken about `System.Random`?

No. `System.Random` is not the reason, and this was never a difference between Windows and Linux. A seeded `System.Random` gives the same sequence on every operating system, as long as the runtime version is the same.

Two independent things came together in 6.0:

* **Per-member seeds are what actually moved the values.** Until 5.x all sources of a session shared one seed. Two members with the same source type produced the same values, and the nullable members of an object became null all at once. Giving every member its own stream derived from the session seed fixes that, and it changes every sequence by definition. The bug fixes above (null thresholds, inclusive date and integer ranges) moved a few more.
* **`StableRandom` is insurance, not a bug fix.** .NET treats the algorithm behind `Random` as an implementation detail and has already replaced it once: .NET 6 moved the unseeded path to xoshiro256** and kept the old algorithm for seeded instances purely for compatibility. Nothing promises that the seeded path survives the next change. "The same seed gives the same test data in five years, on whatever .NET is current then" is not something you can build on `System.Random`, so AutoPoco owns its generator now. Since 6.0 was going to break the sequences anyway, this was the moment to do it.

Afterwards the sequences are frozen by AutoPoco's own code for the whole major version.

For the record, this library did have a real operating system dependency once, but it was not the generator: `CountrySource` read the culture list from the operating system, which made test data differ between developer machines and build pipelines. That was fixed in 4.1.3 with a stable country list; the old source lives on as `CountryFromCultureListSource` and its tests are skipped for exactly this reason.

# New in 5.1.1
* Support for .NET 9 / .NET 10 added

# New in 4.2.2
* RandomUtfTextSource / NullableRandomUtfTextSource added

# New in 4.2.1
* Support for .net8
* Update of several nugets (xunit, moq) to newest version


# New in 4.1.3
* CountrySource as a stable country list (status of 2023) added, support for ISO3-Codes as abbreviation
* CountrySource (old) is now CountryFromCultureListSource. This source is not stable in terms of repeatable test data because it depends on culture list of underlying operation system. This causes tests to fail e. g. in build pipelines.

# New in 4.1.2
* small bugfix for TimeSpan (underflow / overflow) calculation

# New in 4.1.1
* support for typesafe settings via lambda chaining in USE
* support for own factory outside of AutoPoco in USE
* many datasources now support this notation
* now its possible to set the random null evaluator if the standard implementation does not fit
* now its possible to set the nullable threshold value if the standard value (15) does not fit
* net7+ : new Int128Source / NullableInt128Source / Int128IdSource

```CSHARP
_factoryWithComplexRule = AutoPocoContainer.Configure(x => {
         x.Include<SimpleUser>()
            .Setup(c => c.FirstName).Use<FirstNameSource>()
            .Setup(c => c.LastName).Use<LastNameSource>()
            .Setup(c => c.EmailAddress).Use<EmailAddressSource>()
            .Setup(c => c.Id).Use<Int128IdSource>(y => y.SetStartValue(100000))
            .Setup(c => c.ExternalId).Use<Int128Source>()

            // support for external factory
            .Setup(c => c.City).Use<IDataSource<string>>(new StringDataSourceFactory())

            // support for lambdas to configure a datasource
            .Setup(c => c.Birthday).Use<DateOnlySource>(
               x => x.SetMinDate(new DateOnly(1968, 1, 1))
                     .SetMaxDate(new DateOnly(2023, 10, 17))
            )
            .Invoke(c => c.SetPassword(Use.Source<string, PasswordSource>()!));
      });

```

# New in 4.0.1
* most of XYdatasources now have a NullableXYDataSource pendant to get random NULL values instead of normal data back. Helpful for DTOs with nullable fields
* some new datasources, e. g. DateOnlySource, TimeOnlySource, RandomTextSource, LongSource, LongIdSource (and their Nullable pendants)
* RandomStringSOurce now supports a configurable range of chars (e. g. 'A' .. 'z') or a char[] of allowed chars
* some bugfixes regarding index ranges, LINQ replacements for loops
* AutoPocoGlobalSettings introduced, set the properies before every other action / creation to change framework behavior
* Demo project (in tests) updated
* Please note, the CountrySourceTest may fail on your system. It depends on the culture list of your operation system. 

# Breaking change in 4.0.1
* behaviour change: EmailAddressSource, ExtendedEmailAddressSource now generate data according to RFC2606 to make sure that no valid data will be generated
* some more grouping of datasources (primitives, business, country)

# New in 3.7.x
* Ported to newest .net version (at the moment 7)
* Unit tests changed to Xunit (was Nunit)
* Some more unit tests, now all datasources should be covered
* As far as possible test are now consolidated to make testing shorter and more efficient
* Several patches and fixes applied
* Added some more datasources
* All datasources now have a common base class
* Shrinked projects from 4 to 2 (implementation and tests)
* Several improvements regarding code
* Demo project (in tests) updated

# Changes in 3.7.2

Breaking change: Reorganized datasources

* base
* primitives
* business

We plan to add more datasources in future and so we want to provide a better overview over datasources


# Behavior change (maybe, only some datasources)
All datasources reproduce same results within same seed - important for testing. Since 6.0 the seed of a factory is set with `x.UseSeed(seed)` and a single session can get another one with `CreateSession(recursionLimit, seed)`, see "New in 6.0.0". Additionally you can use

* DataSource.SetSeedToRandomValue()
* DataSource.SetSeedToRandomValue(seed)

on a single source; a seed set this way wins over the session seed.

# AutoPoco.NetCore
AutoPoco is a highly configurable framework for the purpose of fluently building readable test data from Plain Old CLR Objects. This is a modernized (i.e. dotnet core 2.0.0) port of the excellent AutoPoco project which hasn't been maintained since 2014. Thanks to [@robashton](https://twitter.com/robashton) for graciously agreeing to let me pull this fossil out of the amber and give it a light dusting.

## What does this do?

AutoPoco replaces manually written object mothers/test data builders with a fluent interface and an easy way to generate a large amount of readable test data. By default, no manual set-up is required, conventions can then be written against the names/types of property or manual configuration can be used against specific objects. 

The primary use cases are 
* Creating single, valid objects for unit tests in a standard manner across all tests 
* Creating large amounts of valid test data for database population

The default data sources and convention is to perform set up once on start-up, and create a 'generation session' for each test that is run, every session will generate repeatable test data so no flickering tests.

``` CSharp
// Perform factory set up (once for entire test run)
IGenerationSessionFactory factory = AutoPocoContainer.Configure(x =>
{
    x.Conventions(c =>
    {
        c.UseDefaultConventions();
    });
    x.AddFromAssemblyContainingType<SimpleUser>();
});

// Generate one of these per test (factory will be a static variable most likely)
IGenerationSession session = factory.CreateSession();

// Get a single user
SimpleUser user = session.Single<SimpleUser>().Get();

// Get a collection of users
List<SimpleUser> users = session.List<SimpleUser>(100).Get();

// Get a collection of users, but set their role manually
mSession.List<SimpleUser>(10)
               .Impose(x => x.Role, sharedRole)
               .Get();
```
Head to the [Documentation](https://github.com/reddy6ue/AutoPoco.NetCore/wiki) to get started.

## Features ##
  * Generates the same pseudo-random data per session, so your tests don't "flicker" 
  * Convention based configuration to set up defaults on un-specified properties/fields 
  * Convention based configuration to match names/types on properties/fields that match a certain pattern 
  * Fluent configuration to set up data sources for meaningful data such as e-mail addresses/names 
  * Fluent interfaces to generate lists/single objects with all properties automatically populated 
  * Ability to override conventions + configuration at object-creation time with manually specified values 
  * Support for method invocation as part of an object's initialisation both automatically via configuration and at object-creation time 
  * Support for the inheritance of rules from base classes/interfaces
  
  # Quick Example #
  
  ## Member Conventions ##
  Define a rule for all string properties called EmailAddress
  ``` CSharp
  public class EmailAddressPropertyConvention : ITypePropertyConvention
{
    public void Apply(ITypePropertyConventionContext context)
    {
        context.SetSource<EmailAddressSource>();
    }

    public void SpecifyRequirements(ITypeMemberConventionRequirements requirements)
    {
        requirements.Name(x => String.Compare(x, "EmailAddress", true) == 0);
        requirements.Type(x => x == typeof(String));
    }
}       
```

## Optionally Setting up data sources for an object ##
Make the properties get their data from the specified sources on creation
Call the SetPassword method with a password source on creation

``` CSharp
  x.Include<SimpleUser>()
    .Setup(c => c.EmailAddress).Use<EmailAddressSource>()
    .Setup(c => c.FirstName).Use<FirstNameSource>()
    .Setup(c => c.LastName).Use<LastNameSource>()
    .Invoke(c => c.SetPassword(
        Use.Source<String, PasswordSource>()));
```

## Simple Object Creation ##
Create a user populated with defaults - all properties will be set (including other complex objects)

``` CSharp
  SimpleUser user = mSession.Single<SimpleUser>().Get();
```

## More Complex Object Creation ##
Create a role for a collection of users and ask for a list of users using that single role
``` CSharp
           SimpleUserRole sharedRole = mSession.Single<SimpleUserRole>()
               .Impose(x=>x.Name, "Shared Role")
               .Get();

           mSession.List<SimpleUser>(10)
               .Impose(x => x.Role, sharedRole)
               .Get();
```

## Hard Core Object Creation ##
Create three roles
Create 100 users
The first 50 of those users will be called Rob Ashton
The last 50 of those users will be called Luke Smith
25 Random users will have RoleOne
A different 25 random users will have RoleTwo
And the other 50 users will have RoleThree
And set the password on every single user to Password1

``` CSharp
            SimpleUserRole roleOne = mSession.Single<SimpleUserRole>()
                              .Impose(x => x.Name, "RoleOne").Get();
            SimpleUserRole roleTwo = mSession.Single<SimpleUserRole>()
                              .Impose(x => x.Name, "RoleTwo").Get();
            SimpleUserRole roleThree = mSession.Single<SimpleUserRole>()
                              .Impose(x => x.Name, "RoleThree").Get();

            mSession.List<SimpleUser>(100)
                 .First(50)
                      .Impose(x => x.FirstName, "Rob")
                      .Impose(x => x.LastName, "Ashton")
                  .Next(50)
                      .Impose(x => x.FirstName, "Luke")
                      .Impose(x => x.LastName, "Smith")
                  .All().Random(25)
                      .Impose(x => x.Role,roleOne)
                  .Next(25)
                      .Impose(x => x.Role,roleTwo)
                  .Next(50)
                      .Impose(x => x.Role, roleThree)
                 .All()
                      .Invoke(x => x.SetPassword("Password1"))
                 .Get();
```
