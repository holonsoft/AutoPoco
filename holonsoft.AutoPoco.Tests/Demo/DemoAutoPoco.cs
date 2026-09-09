using Shouldly;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Demo;

#pragma warning disable CS8631, CS8620

public class DemoAutoPoco
{

    private static IGenerationSessionFactory _factoryWithDefaults = null!;
    private static IGenerationSessionFactory _factoryWithComplexRule = null!;
    private static IGenerationSessionFactory _factoryWithComplexRuleForRecords = null!;

    private class PasswordSource() : RandomStringSource(5, 20, 'A', 'z');

    public DemoAutoPoco()
    {
        _factoryWithDefaults = AutoPocoContainer.Configure(x =>
        {
            x.Conventions(c =>
            {
                c.UseDefaultConventions();
            });
            x.AddFromAssemblyContainingType<SimpleUser>();
        });

        _factoryWithComplexRule = AutoPocoContainer.Configure(x =>
        {
            x.Include<SimpleUser>()
               .Setup(c => c.FirstName).Use<FirstNameSource>()
               .Setup(c => c.LastName).Use<LastNameSource>()
               .Setup(c => c.EmailAddress).Use<EmailAddressSource>()
               .Setup(c => c.Id).Use<Int128IdSource>(y => y.SetStartValue(100000))
               .Setup(c => c.ExternalId).Use<Int128Source>()
               .Setup(c => c.TimeSpan).Use<TimeSpanSource>()
               // support for external factory
               .Setup(c => c.City).Use<IDataSource<string>>(new StringDataSourceFactory())
               // support for lambdas to configure a datasource
               .Setup(c => c.Birthday).Use<DateOnlySource>(
                  x => x.SetMinDate(new DateOnly(1968, 1, 1))
                                    .SetMaxDate(new DateOnly(2023, 10, 17))
               )
               .Invoke(c => c.SetPassword(Use.Source<string, PasswordSource>()!));
        });

        _factoryWithComplexRuleForRecords = AutoPocoContainer.Configure(x =>
        {
            x.Include<SimpleUserRecord>() // since 6.0 a record needs no parameterless ctor, the configured members go through the primary ctor
               .Setup(c => c.FirstName).Use<FirstNameSource>()
               .Setup(c => c.LastName).Use<LastNameSource>()
               .Setup(c => c.EmailAddress).Use<EmailAddressSource>()
               .Setup(c => c.Id).Use<Int128IdSource>(y => y.SetStartValue(100000))
               .Setup(c => c.ExternalId).Use<Int128Source>()
               .Setup(c => c.City).Use<IDataSource<string>>(new StringDataSourceFactory())
               .Setup(c => c.Birthday).Use<DateOnlySource>(
                  x => x.SetDateRange(new DateOnly(1968, 1, 1), new DateOnly(2023, 10, 17)))
               .Invoke(c => c.SetPassword(Use.Source<string, PasswordSource>()!));
        });
    }

    [Fact]
    public void SomeDemonstrationOfComplexGenerations()
    {
        // Generate one of these per test (factory will be a static variable most likely)
        var session1 = _factoryWithDefaults.CreateSession();

        // Get a single user
        var user = session1
                    .Single<SimpleUser>()
                    .Get();

        // Get a collection of users
        var users = session1
                    .List<SimpleUser>(100)
                    .Get()
                    .ToArray();

        users.Count().ShouldBe(100);

        // Get a collection of users, but set their role manually
        var sharedRole = session1.Single<SimpleUserRole>()
                 .Impose(x => x.Name, "Shared Role")
                 .Get();

        users = session1
                 .List<SimpleUser>(10)
                 .Impose(x => x.Role, sharedRole)
                 .Get()
                 .ToArray();

        var session2 = _factoryWithComplexRule.CreateSession();

        user = session2
                 .Single<SimpleUser>()
                 .Get();

        user.FirstName.ShouldBe("Alexa");
        user.LastName.ShouldBe("Wilson");
        user.RevealedPassword.ShouldBe("DPj`I`FMBd");
        user.ExternalId.ShouldNotBe(0);
        user.Id.ShouldNotBe(0);
        user.City.ShouldNotBeNullOrWhiteSpace();
        user.City.ShouldNotBe("no-city");

        // Create three roles
        // Create 100 users
        // The first 50 of those users will be called Rob Ashton
        // The last 50 of those users will be called Luke Smith
        // 25 Random users will have RoleOne
        // A different 25 random users will have RoleTwo
        // And the other 50 users will have RoleThree
        // And set the password on every single user to Password1
        var roleOne = session2
                       .Single<SimpleUserRole>()
                       .Impose(x => x.Name, "RoleOne").Get();

        var roleTwo = session2
                       .Single<SimpleUserRole>()
                       .Impose(x => x.Name, "RoleTwo").Get();

        var roleThree = session2
                       .Single<SimpleUserRole>()
                       .Impose(x => x.Name, "RoleThree").Get();

        var someUsers = session2
                       .List<SimpleUser>(100)
                         .First(50)
                              .Impose(x => x.FirstName, "Rob")
                              .Impose(x => x.LastName, "Ashton")
                          .Next(50)
                              .Impose(x => x.FirstName, "Luke")
                              .Impose(x => x.LastName, "Smith")
                          .All()
                          .Random(25)
                              .Impose(x => x.Role, roleOne)
                          .Next(25)
                              .Impose(x => x.Role, roleTwo)
                          .Next(50)
                              .Impose(x => x.Role, roleThree)
                         .All()
                              .Invoke(x => x.SetPassword("Password1"))
                         .Get()
                         .ToArray();

        someUsers.Count().ShouldBe(100);
    }

    [Fact]
    public void SomeDemonstrationOfComplexGenerationsWithRecords()
    {
        var session = _factoryWithComplexRuleForRecords.CreateSession();

        var user = session
                 .Single<SimpleUserRecord>()
                 .Get();

        user.FirstName.ShouldBe("Arthur");
        user.LastName.ShouldBe("Walker");
        user.RevealedPassword.ShouldBe("_AqPqoJvdRvaHXmjJK");

        // Create three roles
        // Create 100 users
        // The first 50 of those users will be called Rob Ashton
        // The last 50 of those users will be called Luke Smith
        // 25 Random users will have RoleOne
        // A different 25 random users will have RoleTwo
        // And the other 50 users will have RoleThree
        // And set the password on every single user to Password1
        var roleOne = session
                       .Single<SimpleUserRoleRecord>()
                       .Impose(x => x.Name, "RoleOne").Get();

        var roleTwo = session
                       .Single<SimpleUserRoleRecord>()
                       .Impose(x => x.Name, "RoleTwo").Get();

        var roleThree = session
                       .Single<SimpleUserRoleRecord>()
                       .Impose(x => x.Name, "RoleThree").Get();

        roleOne.Name.ShouldBe("RoleOne");
        roleTwo.Name.ShouldBe("RoleTwo");
        roleThree.Name.ShouldBe("RoleThree");

        var someUsers = session
                       .List<SimpleUserRecord>(100)
                         .First(50)
                              .Impose(x => x.FirstName, "Rob")
                              .Impose(x => x.LastName, "Ashton")
                          .Next(50)
                              .Impose(x => x.FirstName, "Luke")
                              .Impose(x => x.LastName, "Smith")
                          .All()
                          .Random(25)
                              .Impose(x => x.Role, roleOne)
                          .Next(25)
                              .Impose(x => x.Role, roleTwo)
                          .Next(50)
                              .Impose(x => x.Role, roleThree)
                         .All()
                              .Invoke(x => x.SetPassword("Password1"))
                         .Get()
                         .ToArray();

        someUsers.Count().ShouldBe(100);
    }
}