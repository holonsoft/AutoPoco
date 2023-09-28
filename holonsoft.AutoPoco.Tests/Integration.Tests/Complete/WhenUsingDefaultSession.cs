using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

// This is the Scott Hanselman Class
public class WhenUsingDefaultSession {
   [Fact]
   public void NextReturnsValidObject() {
      var session = AutoPocoContainer.CreateDefaultSession();
      var user = session.Next<SimpleUser>();

      user.Should().NotBeNull("User was not created");
      user.FirstName.Should().NotBeNull("User did not get first name");
   }

   [Fact]
   public void NextWithActionAllowsConfigurationOfObject() {
      var session = AutoPocoContainer.CreateDefaultSession();
      var user = session.Next<SimpleUser>(all => {
         all.Impose(x => x.FirstName, "Scott");
         all.Impose(x => x.LastName, "Hanselman");
      });

      user.FirstName.Should().Be("Scott");
      user.LastName.Should().Be("Hanselman");
   }

   [Fact]
   public void NextWithActionAndNumberAllowsConfigurationOfObjects() {
      var session = AutoPocoContainer.CreateDefaultSession();
      var users = session.Collection<SimpleUser>(10, all => {
         all.Random(5)
           .Impose(x => x.FirstName, "Rob")
           .Next(5)
           .Impose(x => x.FirstName, "Scott");

         all.Random(5)
           .Impose(x => x.LastName, "Red")
           .Next(5)
           .Impose(x => x.LastName, "Blue");
      }).ToList();

      users.Count(x => x.FirstName == "Rob").Should().Be(5);
      users.Count(x => x.FirstName == "Scott").Should().Be(5);
      users.Count(x => x.LastName == "Red").Should().Be(5);
      users.Count(x => x.LastName == "Blue").Should().Be(5);
   }

   [Fact]
   public void NextWithNumberReturnsCollectionOfValidObjects() {
      var session = AutoPocoContainer.CreateDefaultSession();
      var users = session.Collection<SimpleUser>(10).ToList();

      users.ForEach(x => {
         x.Should().NotBeNull("User was not created");
         x.FirstName.Should().NotBeNull("User did not get first name");
      });
   }
}