using Shouldly;
using Moq;
using System.Linq.Expressions;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class IndexAwareImposeTests {
   private static readonly Expression<Func<SimpleUser, string>> _lastName = x => x.LastName;

   private static List<Mock<IObjectGenerator<SimpleUser>>> Mocks(int count)
      => Enumerable.Range(0, count).Select(_ => new Mock<IObjectGenerator<SimpleUser>>()).ToList();

   [Fact]
   public void CollectionImposeWithIndexPassesThePositionOfEveryItem() {
      var mocks = Mocks(5);
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(mocks.Select(m => m.Object));

      context.Impose(_lastName, i => $"item-{i}").ShouldBeSameAs(context);

      for (var i = 0; i < 5; i++)
         mocks[i].Verify(m => m.Impose(_lastName, $"item-{i}"), Times.Once());
   }

   [Fact]
   public void CollectionImposeWithIndexAndItemRegistersAFactoryPerItem() {
      var mocks = Mocks(3);
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(mocks.Select(m => m.Object));
      var captured = new List<Func<SimpleUser, string>>();
      foreach (var mock in mocks)
         mock.Setup(m => m.Impose(_lastName, It.IsAny<Func<SimpleUser, string>>()))
            .Callback((Expression<Func<SimpleUser, string>> _, Func<SimpleUser, string> f) => captured.Add(f));

      context.Impose(_lastName, (i, user) => $"{user.FirstName}-{i}");

      captured.Count.ShouldBe(3);
      var user = new SimpleUser { FirstName = "Rob", LastName = "", EmailAddress = "", Role = new SimpleUserRole { Name = "" }, Properties = [] };
      captured.Select(f => f(user)).ShouldBe(["Rob-0", "Rob-1", "Rob-2"]);
   }

   [Fact]
   public void SelectionImposeWithIndexUsesThePositionInTheWholeCollectionAlsoAfterRandom() {
      var mocks = Mocks(6);
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(mocks.Select(m => m.Object));

      context.Random(6).Impose(_lastName, i => $"pos-{i}");

      for (var i = 0; i < 6; i++)
         mocks[i].Verify(m => m.Impose(_lastName, $"pos-{i}"), Times.Once());
   }

   [Fact]
   public void SelectionImposeWithIndexOnlyTouchesTheSelection() {
      var mocks = Mocks(6);
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(mocks.Select(m => m.Object));

      context.First(2).Next(2).Impose(_lastName, i => $"pos-{i}");

      mocks[0].Verify(m => m.Impose(_lastName, It.IsAny<string>()), Times.Never());
      mocks[2].Verify(m => m.Impose(_lastName, "pos-2"), Times.Once());
      mocks[3].Verify(m => m.Impose(_lastName, "pos-3"), Times.Once());
      mocks[5].Verify(m => m.Impose(_lastName, It.IsAny<string>()), Times.Never());
   }

   [Fact]
   public void SelectionImposeWithIndexAndItemRegistersAFactory() {
      var mocks = Mocks(2);
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(mocks.Select(m => m.Object));

      context.First(2).Impose(_lastName, (i, user) => $"{i}");

      mocks[0].Verify(m => m.Impose(_lastName, It.IsAny<Func<SimpleUser, string>>()), Times.Once());
      mocks[1].Verify(m => m.Impose(_lastName, It.IsAny<Func<SimpleUser, string>>()), Times.Once());
   }

   [Fact]
   public void NullFactoriesAreRejected() {
      var context = new CollectionContext<SimpleUser, List<SimpleUser>>(Mocks(1).Select(m => m.Object));

      Should.Throw<ArgumentNullException>(() => context.Impose(_lastName, (Func<int, string>) null!));
      Should.Throw<ArgumentNullException>(() => context.Impose(_lastName, (Func<int, SimpleUser, string>) null!));
      Should.Throw<ArgumentNullException>(() => context.First(1).Impose(_lastName, (Func<int, string>) null!));
      Should.Throw<ArgumentNullException>(() => context.First(1).Impose(_lastName, (Func<int, SimpleUser, string>) null!));
   }

   [Fact]
   public void MemberSetFromFuncActionSetsPropertiesAndFields() {
      var propertyAction = new ObjectMemberSetFromFuncAction<SimpleUserRole, string>(
         new EngineTypePropertyMember(typeof(SimpleUserRole).GetProperty(nameof(SimpleUserRole.Name))!), x => x.Name + "p");
      var fieldAction = new ObjectMemberSetFromFuncAction<SimpleFieldClass, string?>(
         new EngineTypeFieldMember(typeof(SimpleFieldClass).GetField(nameof(SimpleFieldClass.SomeField))!), x => x.SomeOtherField + "f");
      var role = new SimpleUserRole { Name = "role" };
      var fields = new SimpleFieldClass { SomeOtherField = "other" };

      propertyAction.Enact(null, role);
      fieldAction.Enact(null, fields);

      role.Name.ShouldBe("rolep");
      fields.SomeField.ShouldBe("otherf");
   }

   [Fact]
   public void MemberSetFromFuncActionRejectsMethodsAndNulls() {
      var method = new EngineTypeMethodMember(typeof(SimpleUser).GetMethod(nameof(SimpleUser.SetPassword))!);

      Should.Throw<ArgumentException>(() => new ObjectMemberSetFromFuncAction<SimpleUser, string>(method, x => ""));
      Should.Throw<ArgumentNullException>(() => new ObjectMemberSetFromFuncAction<SimpleUser, string>(null!, x => ""));
      Should.Throw<ArgumentNullException>(() => new ObjectMemberSetFromFuncAction<SimpleUser, string>(
         new EngineTypePropertyMember(typeof(SimpleUser).GetProperty(nameof(SimpleUser.LastName))!), null!));
   }
}
