using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingSessionSeeds {
   private static IGenerationSessionFactory CreateFactory(int? seed = null, bool withLastName = true)
      => AutoPocoContainer.Configure(x => {
         if (seed.HasValue)
            x.UseSeed(seed.Value);

         var user = x.Include<TwoNullableNumbersClass>()
            .Setup(c => c.First).Use<NullableIntegerSource>()
            .Setup(c => c.Second).Use<NullableIntegerSource>()
            .Setup(c => c.FirstName).Use<FirstNameSource>();

         if (withLastName)
            user.Setup(c => c.LastName).Use<FirstNameSource>();
      });

   private static List<TwoNullableNumbersClass> Generate(IGenerationSession session, int count = 50)
      => session.Collection<TwoNullableNumbersClass>(count).ToList();

   private static List<string?> FirstNames(IGenerationSession session, int count = 50)
      => Generate(session, count).Select(x => x.FirstName).ToList();

   [Fact]
   public void TwoSessionsOfTheSameFactoryProduceTheSameData() {
      var factory = CreateFactory();

      FirstNames(factory.CreateSession()).ShouldBe(FirstNames(factory.CreateSession()));
   }

   [Fact]
   public void TwoFactoriesWithTheSameSeedProduceTheSameData()
      => FirstNames(CreateFactory(42).CreateSession()).ShouldBe(FirstNames(CreateFactory(42).CreateSession()));

   [Fact]
   public void TheDefaultSeedIsTheLibraryDefault() {
      FirstNames(CreateFactory().CreateSession()).ShouldBe(FirstNames(CreateFactory(AutoPocoDefaults.Seed).CreateSession()));
      ((GenerationContext) CreateFactory().CreateSession()).Builders.Seed.ShouldBe(AutoPocoDefaults.Seed);
   }

   [Fact]
   public void ADifferentSeedProducesDifferentData()
      => FirstNames(CreateFactory(1).CreateSession()).ShouldNotBe(FirstNames(CreateFactory(2).CreateSession()));

   [Fact]
   public void ASessionSeedOverridesTheFactorySeed() {
      var fromFactorySeed = FirstNames(CreateFactory(42).CreateSession());
      var fromSessionSeed = FirstNames(CreateFactory(7).CreateSession(AutoPocoDefaults.RecursionLimit, 42));

      fromSessionSeed.ShouldBe(fromFactorySeed);
      ((GenerationContext) CreateFactory(7).CreateSession(3, 42)).Builders.Seed.ShouldBe(42);
   }

   [Fact]
   public void TwoMembersWithTheSameSourceTypeGetDifferentValues() {
      var items = Generate(CreateFactory().CreateSession());

      items.Select(x => x.FirstName).ShouldNotBe(items.Select(x => x.LastName));
   }

   [Fact]
   public void NullableMembersDoNotBecomeNullTogether() {
      var items = Generate(CreateFactory().CreateSession(), 300);

      items.Count(x => x.First == null).ShouldBeInRange(20, 80);
      items.Count(x => x.Second == null).ShouldBeInRange(20, 80);
      items.ShouldContain(x => x.First == null && x.Second != null);
      items.ShouldContain(x => x.First != null && x.Second == null);
   }

   [Fact]
   public void TheValuesOfAMemberDoNotDependOnOtherMembers()
      => FirstNames(CreateFactory(withLastName: false).CreateSession()).ShouldBe(FirstNames(CreateFactory(withLastName: true).CreateSession()));

   [Fact]
   public void AnExplicitSeedInTheConfigurationWinsOverTheSessionSeed() {
      var session = AutoPocoContainer.Configure(x => {
         x.UseSeed(99);
         x.Include<TwoNullableNumbersClass>()
            .Setup(c => c.First).Use<NullableIntegerSource>(s => s.SetSeedToRandomValue(7));
      }).CreateSession();
      var standalone = new NullableIntegerSource();
      standalone.SetSeedToRandomValue(7);

      Generate(session, 10).Select(x => x.First).ShouldBe(Enumerable.Range(0, 10).Select(_ => standalone.Next(null)));
   }

   [Fact]
   public void AGenerationTimeSourceSharedByAListIsSeededOnceAndKeepsRunning() {
      var names = CreateFactory().CreateSession()
         .List<TwoNullableNumbersClass>(20)
         .Source(x => x.LastName, new LastNameSource())
         .Get()
         .Select(x => x.LastName)
         .ToList();

      names.Distinct().Count().ShouldBeGreaterThan(5);
   }

   [Fact]
   public void ARandomSelectionIsTheSameForTheSameSeed() {
      static List<int> SelectedIndexes(IGenerationSession session) {
         var items = session.List<TwoNullableNumbersClass>(30)
            .Random(10).Impose(x => x.First, -1)
            .All()
            .Get();
         return items.Select((x, i) => (x, i)).Where(t => t.x.First == -1).Select(t => t.i).ToList();
      }

      SelectedIndexes(CreateFactory(5).CreateSession()).ShouldBe(SelectedIndexes(CreateFactory(5).CreateSession()));
      SelectedIndexes(CreateFactory(5).CreateSession()).ShouldNotBe(SelectedIndexes(CreateFactory(6).CreateSession()));
   }

   [Fact]
   public void ConstructorArgumentsFollowTheSeedAsWell() {
      static List<string> Names(int seed)
         => AutoPocoContainer.Configure(x => {
            x.UseSeed(seed);
            x.Include<ImmutableRoleRecord>().Setup(c => c.Name).Use<FirstNameSource>();
         }).CreateSession().Collection<ImmutableRoleRecord>(20).Select(r => r.Name).ToList();

      Names(1).ShouldBe(Names(1));
      Names(1).ShouldNotBe(Names(2));
   }

   [Fact]
   public void NullableAnnotationPatternsFollowTheSeed() {
      static List<bool> NullPattern(int seed)
         => AutoPocoContainer.Configure(x => {
            x.UseSeed(seed);
            x.RespectNullableAnnotations(50);
            x.Include<TwoNullableNumbersClass>().Setup(c => c.FirstName).Use<FirstNameSource>();
         }).CreateSession().Collection<TwoNullableNumbersClass>(40).Select(r => r.FirstName == null).ToList();

      NullPattern(1).ShouldBe(NullPattern(1));
      NullPattern(1).ShouldNotBe(NullPattern(2));
   }

   [Fact]
   public void TheConfigurationBuilderStoresTheSeed() {
      var builder = new EngineConfigurationBuilder();
      builder.Seed.ShouldBe(AutoPocoDefaults.Seed);

      ((IEngineConfigurationBuilder) builder).UseSeed(4711);

      builder.Seed.ShouldBe(4711);
   }
}
