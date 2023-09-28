# holonsoft / AutoPoco
* Ported to newest .net version (at the moment 7)
* Unit tests changed to Xunit (was Nunit)
* Some more unit tests, now all datasources should be covered
* As far as possible test are now consolidated to make testing shorter and more efficient
* Several patches and fixes applied
* Added some more datasources
* All datasources now have a common base class
* Shrinked projects from 4 to 2 (implementation and tests)
* Several improvements regarding code


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
