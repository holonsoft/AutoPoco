using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Extensions;

public class GenerationExtensionsTests {
   public class Child { public string Name { get; set; } = string.Empty; }

   public class Poco {
      public string Name { get; set; } = string.Empty;
      public int Number { get; set; }
      public int NumberField;
      public Child? Kid { get; set; }
   }

   private static IGenerationSession NewSession()
      => AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>();
         x.Include<Child>();
      }).CreateSession();

   [Fact]
   public void SingleUsesTheLambdaForAProperty() {
      var poco = NewSession().Single<Poco>().Source(p => p.Name, () => "from lambda").Get();
      poco.Name.ShouldBe("from lambda");
   }

   [Fact]
   public void SingleUsesTheLambdaForAField() {
      var poco = NewSession().Single<Poco>().Source(p => p.NumberField, () => 77).Get();
      poco.NumberField.ShouldBe(77);
   }

   [Fact]
   public void SingleLambdaReceivesTheContext() {
      var poco = NewSession().Single<Poco>()
         .Source(p => p.Kid, ctx => ctx.Single<Child>().Impose(c => c.Name, "built in lambda").Get())
         .Get();

      poco.Kid.ShouldNotBeNull();
      poco.Kid!.Name.ShouldBe("built in lambda");
   }

   [Fact]
   public void ListRunsTheLambdaOncePerItem() {
      var counter = 0;
      var items = NewSession().List<Poco>(5).Source(p => p.Number, () => ++counter).Get();

      items.Select(p => p.Number).ShouldBe([1, 2, 3, 4, 5]);
   }

   [Fact]
   public void ListLambdaReceivesTheContext() {
      var items = NewSession().List<Poco>(3)
         .Source(p => p.Kid, ctx => ctx.Single<Child>().Impose(c => c.Name, "kid").Get())
         .Get();

      items.ShouldNotBeEmpty();
      items.ShouldAllBe(p => p.Kid != null && p.Kid.Name == "kid");
      items.Select(p => p.Kid).Distinct().Count().ShouldBe(3, "every item gets its own child instance");
   }

   [Fact]
   public void LambdaOverrideWinsOverConfiguredSource() {
      var session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<Poco>().Setup(p => p.Name).Value("configured");
      }).CreateSession();

      var configured = session.Single<Poco>().Get();
      var overridden = session.Single<Poco>().Source(p => p.Name, () => "overridden").Get();

      configured.Name.ShouldBe("configured");
      overridden.Name.ShouldBe("overridden");
   }
}
