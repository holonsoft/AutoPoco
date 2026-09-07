using System.Collections;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Extensions;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Extensions;

public class StandardExtensionsTests {
   public class Child { public string? Name { get; set; } }

   public class Poco {
      public string Name { get; set; } = string.Empty;
      public int Number { get; set; }
      public int? MaybeNumber { get; set; }
      public Child? Kid { get; set; }
      public List<Child> Children { get; set; } = new();
      public ArrayList Untyped { get; set; } = new();
   }

   [Fact]
   public void FromRunsTheLambdaOncePerGeneratedObject() {
      var counter = 0;
      var nameCounter = 0;
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>()
            .Setup(c => c.Number).From(() => ++counter)
            .Setup(c => c.Name).From(() => $"User {++nameCounter}");
      }).CreateSession();

      var items = session.List<Poco>(4).Get();

      items.Select(p => p.Number).ShouldBe([1, 2, 3, 4]);
      items.Select(p => p.Name).ShouldBe(["User 1", "User 2", "User 3", "User 4"]);
   }

   [Fact]
   public void FromLeavesNullDecisionsToTheLambda() {
      var toggle = false;
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>().Setup(c => c.MaybeNumber).From(() => (toggle = !toggle) ? 1 : null);
      }).CreateSession();

      var values = session.Collection<Poco>(6).Select(p => p.MaybeNumber).ToList();

      values.ShouldBe([1, null, 1, null, 1, null]);
   }

   [Fact]
   public void FromWithContextCanBuildRelatedObjects() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Child>();
         x.Include<Poco>().Setup(c => c.Kid).From(ctx => ctx!.Single<Child>().Impose(k => k.Name, "kid").Get());
      }).CreateSession();

      var poco = session.Single<Poco>().Get();

      poco.Kid.ShouldNotBeNull();
      poco.Kid!.Name.ShouldBe("kid");
   }

   [Fact]
   public void ValueImposesAFixedValueOnEveryInstance() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>()
            .Setup(c => c.Name).Value("fixed")
            .Setup(c => c.Number).Value(42);
      }).CreateSession();

      var items = session.List<Poco>(5).Get();

      items.Count().ShouldBe(5);
      items.ShouldAllBe(p => p.Name == "fixed" && p.Number == 42);
   }

   [Fact]
   public void RandomStringHonorsTheLengthBounds() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>().Setup(c => c.Name).Random(3, 6);
      }).CreateSession();

      var names = session.Collection<Poco>(50).Select(p => p.Name).ToList();

      names.ShouldAllBe(n => n != null && n.Length >= 3 && n.Length <= 6);
      names.Distinct().Count().ShouldBeGreaterThan(1);
   }

   [Fact]
   public void CollectionFillsAGenericListWithinTheBounds() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>().Setup(c => c.Children).Collection(2, 4);
      }).CreateSession();

      var items = session.Collection<Poco>(20).ToList();

      items.ShouldAllBe(p => p.Children != null && p.Children.Count >= 2 && p.Children.Count <= 4);
      items.SelectMany(p => p.Children!).ShouldAllBe(c => c != null);
   }

   [Fact]
   public void CollectionRejectsNonGenericCollections() {
      Action act = () => AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>().Setup(c => c.Untyped).Collection(1, 2);
      });

      Should.Throw<ArgumentException>(act);
   }
}
