using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenCreatingACustomisedList {
   public WhenCreatingACustomisedList() {
      // As default as it gets
      _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.AddFromAssemblyContainingType<SimpleUser>();
      })
        .CreateSession();

      var roleOne = _session.Single<SimpleUserRole>()
        .Impose(x => x.Name, "RoleOne").Get();
      var roleTwo = _session.Single<SimpleUserRole>()
        .Impose(x => x.Name, "RoleTwo").Get();
      var roleThree = _session.Single<SimpleUserRole>()
        .Impose(x => x.Name, "RoleThree").Get();

      _users = _session.List<SimpleUser>(100)
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
        .All().Get();
   }

   private readonly IGenerationSession _session;
   private readonly IList<SimpleUser> _users;

   [Fact]
   public void CorrectNumberOfRobsExist()
      => _users.Count(x => x.FirstName == "Rob").Should().Be(50);

   [Fact]
   public void CorrectNumberOfAshtonsExist()
      => _users.Count(x => x.LastName == "Ashton").Should().Be(50);

   [Fact]
   public void CorrectNumberOfLukesExist()
      => _users.Count(x => x.FirstName == "Luke").Should().Be(50);

   [Fact]
   public void CorrectNumberOfSmithsExist()
      => _users.Count(x => x.LastName == "Smith").Should().Be(50);

   [Fact]
   public void AllRobsAreAshtons()
      => _users.Count(x => x.FirstName == "Rob" && x.LastName == "Ashton").Should().Be(50);

   [Fact]
   public void AllLukesAreSmiths()
      => _users.Count(x => x.FirstName == "Luke" && x.LastName == "Smith").Should().Be(50);

   [Fact]
   public void CorrectNumberOfRoleOnesExist()
      => _users.Count(x => x.Role.Name == "RoleOne").Should().Be(25);

   [Fact]
   public void CorrectNumberOfRoleTwosExist()
      => _users.Count(x => x.Role.Name == "RoleTwo").Should().Be(25);

   [Fact]
   public void CorrectNumberOfRoleThreesExist()
      => _users.Count(x => x.Role.Name == "RoleThree").Should().Be(50);
}