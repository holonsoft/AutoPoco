# holonsoft / AutoPoco

[![CI](https://github.com/holonsoft/AutoPoco/actions/workflows/ci.yml/badge.svg)](https://github.com/holonsoft/AutoPoco/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/holonsoft.AutoPoco.svg)](https://www.nuget.org/packages/holonsoft.AutoPoco/)

AutoPoco is a highly configurable framework for the purpose of fluently building readable (test) data.
holonsoft ported this famous lib to newest version of dotnet

# New in 6.0.0 (in progress)
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
      .Setup(c => c.Role).From(ctx => ctx!.Single<SimpleUserRole>().Impose(r => r.Name, "Guest").Get());
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
* `IDataSource<T>` is covariant now, so a source of `string` (e.g. `CitySource`) can be used for a `string?` member without a nullability warning.
* Bug fix: every `Nullable*` source ignored an explicit null creation threshold. The fixed array and dictionary based sources (names, companies, cities, capitals, countries, states, zip codes, urls) even used the threshold as their random seed. The threshold is honored now. As a consequence the null positions in the stable sequences of these sources changed, the data itself comes in the same order as before.
* Bug fix: `DateTimeSource`, `DateOnlySource`, `TimeOnlySource` and `DateOfBirthSource` never produced the upper end of their ranges: no December, no 31st, no 23:00, no minute or second 59, and the maximum year of a date of birth was never reached. Ranges within a single year could produce values outside the range. All four sources now pick uniformly from the whole range, both bounds inclusive, and throw an `ArgumentOutOfRangeException` when the maximum lies before the minimum. `TimeOnlySource` supports ranges that wrap around midnight (22:00 to 02:00). `DateTimeSource` keeps the `DateTimeKind` of the minimum date. The generated sequences for a given seed changed.
* Bug fix: `RandomUtfTextSource` could loop forever when it hit a Unicode block without any allowed character.
* Build and packaging: GitHub Actions CI, trusted publishing to nuget.org, MinVer versioning from git tags, central package management, tests on xunit.v3 for net8/9/10
* The library no longer drags FluentAssertions and Moq into your project as dependencies

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
All datasources reproduce same results within same seed - important for testing. Additionally you can use

* DataSource.SetSeedToRandomValue()
* DataSource.SetSeedToRandomValue(seed)

to change behavior or get other values as the internal seed constant generates.

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
