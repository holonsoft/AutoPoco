using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenImposingByIndex {
   private static IGenerationSession CreateSession()
      => AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<SimpleUser>()
            .Setup(c => c.FirstName).Use<FirstNameSource>()
            .Setup(c => c.LastName).Use<LastNameSource>();
      }).CreateSession();

   [Fact]
   public void TheIndexGivesEveryItemItsOwnValue() {
      var users = CreateSession().List<SimpleUser>(10)
         .Impose(u => u.Id, i => (Int128) (i + 1))
         .Impose(u => u.City, i => $"City {i}")
         .Get();

      users.Select(u => u.Id).ShouldBe(Enumerable.Range(1, 10).Select(i => (Int128) i));
      users.Select(u => u.City).ShouldBe(Enumerable.Range(0, 10).Select(i => $"City {i}"));
   }

   [Fact]
   public void TheItemFactorySeesTheGeneratedMembers() {
      var users = CreateSession().List<SimpleUser>(5)
         .Impose(u => u.EmailAddress, (i, u) => $"{u.FirstName}.{u.LastName}.{i}@example.test".ToLowerInvariant())
         .Get();

      users.ShouldAllBe(u => u.EmailAddress.StartsWith(u.FirstName.ToLowerInvariant() + "."));
      users.Select(u => u.EmailAddress).ShouldBe(users.Select((u, i) => $"{u.FirstName}.{u.LastName}.{i}@example.test".ToLowerInvariant()));
   }

   [Fact]
   public void TheItemFactorySeesEarlierImposedValues() {
      var users = CreateSession().List<SimpleUser>(3)
         .Impose(u => u.FirstName, "Rob")
         .Impose(u => u.EmailAddress, (i, u) => $"{u.FirstName}{i}@example.test")
         .Get();

      users.Select(u => u.EmailAddress).ShouldBe(["Rob0@example.test", "Rob1@example.test", "Rob2@example.test"]);
   }

   [Fact]
   public void ASelectionUsesThePositionInTheWholeList() {
      var users = CreateSession().List<SimpleUser>(10)
         .First(3).Impose(u => u.City, i => $"first {i}")
         .Next(3).Impose(u => u.City, (i, u) => $"next {i}")
         .All()
         .Get();

      users.Take(3).Select(u => u.City).ShouldBe(["first 0", "first 1", "first 2"]);
      users.Skip(3).Take(3).Select(u => u.City).ShouldBe(["next 3", "next 4", "next 5"]);
      users.Skip(6).ShouldNotBeEmpty();
      users.Skip(6).ShouldAllBe(u => u.City != null && !u.City.StartsWith("first") && !u.City.StartsWith("next"));
   }

   [Fact]
   public void ARandomSelectionKeepsTheOriginalPositions() {
      var users = CreateSession().List<SimpleUser>(20)
         .Random(8).Impose(u => u.Id, i => (Int128) i)
         .Impose(u => u.City, i => "picked")
         .All()
         .Get();

      var picked = users.Select((u, i) => (u, i)).Where(t => t.u.City == "picked").ToList();
      picked.Count.ShouldBe(8);
      picked.ShouldAllBe(t => t.u.Id == t.i);
   }

   [Fact]
   public void ASingleGeneratorCanImposeFromTheObject() {
      var user = CreateSession().Single<SimpleUser>()
         .Impose(u => u.LastName, "Ashton")
         .Impose(u => u.EmailAddress, u => $"{u.FirstName}.{u.LastName}@example.test")
         .Get();

      user.EmailAddress.ShouldBe($"{user.FirstName}.Ashton@example.test");
   }

   [Fact]
   public void FieldsWorkAsWell() {
      var items = CreateSession().List<SimpleFieldClass>(4)
         .Impose(c => c.SomeField, i => $"field {i}")
         .Get();

      items.Select(c => c.SomeField).ShouldBe(["field 0", "field 1", "field 2", "field 3"]);
   }

   [Fact]
   public void TheClassicImposeStillWorksNextToTheNewOverloads() {
      var users = CreateSession().List<SimpleUser>(3)
         .Impose(u => u.FirstName, "Rob")
         .Impose(u => u.Id, i => (Int128) i)
         .Get();

      users.ShouldAllBe(u => u.FirstName == "Rob");
      users.Select(u => u.Id).ShouldBe([(Int128) 0, 1, 2]);
   }
}
