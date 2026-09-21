using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Faker.DataSources;

namespace holonsoft.AutoPoco.Faker.Tests;

public class Customer {
   public string FirstName { get; set; } = "";
   public string SecondFirstName { get; set; } = "";
}

/// <summary>
///   The Faker sources have to behave like every other AutoPoco source inside a session: the session seed
///   decides the values, two sessions of the same factory agree, and two members of the same type draw from
///   their own stream.
/// </summary>
public class WhenUsingFakerSourcesInASession {
   private static IGenerationSessionFactory CreateFactory(int? seed = null, string? locale = null)
      => AutoPocoContainer.Configure(x => {
         if (seed.HasValue)
            x.UseSeed(seed.Value);

         x.Include<Customer>()
            .Setup(c => c.FirstName).Use<FakerFirstNameSource>(locale!)
            .Setup(c => c.SecondFirstName).Use<FakerFirstNameSource>(locale!);
      });

   private static List<Customer> Generate(IGenerationSession session, int count = 50)
      => session.Collection<Customer>(count).ToList();

   [Fact]
   public void EveryMemberGetsARealValue()
      => Generate(CreateFactory().CreateSession())
         .ShouldAllBe(c => !string.IsNullOrWhiteSpace(c.FirstName) && !string.IsNullOrWhiteSpace(c.SecondFirstName));

   [Fact]
   public void TwoFactoriesWithTheSameSeedProduceTheSameData() {
      var first = Generate(CreateFactory(4711).CreateSession()).Select(c => c.FirstName);
      var second = Generate(CreateFactory(4711).CreateSession()).Select(c => c.FirstName);

      first.ShouldBe(second);
   }

   [Fact]
   public void ADifferentSeedProducesDifferentData() {
      var first = Generate(CreateFactory(1).CreateSession()).Select(c => c.FirstName);
      var second = Generate(CreateFactory(2).CreateSession()).Select(c => c.FirstName);

      first.ShouldNotBe(second);
   }

   [Fact]
   public void TwoSessionsOfTheSameFactoryProduceTheSameData() {
      var factory = CreateFactory(99);

      Generate(factory.CreateSession()).Select(c => c.FirstName)
         .ShouldBe(Generate(factory.CreateSession()).Select(c => c.FirstName));
   }

   /// <summary>
   ///   Two members of the same source type get their own seed from <c>SeedDerivation</c>. Sharing one faker
   ///   would give both members the same name in every object.
   /// </summary>
   [Fact]
   public void TwoMembersOfTheSameSourceTypeGetTheirOwnStream() {
      var customers = Generate(CreateFactory().CreateSession());

      customers.Select(c => c.FirstName).ShouldNotBe(customers.Select(c => c.SecondFirstName));
   }

   /// <summary>
   ///   A session seed has to reach Bogus. If it did not, the library default seed would win and every seed
   ///   would give the same names.
   /// </summary>
   [Fact]
   public void TheSessionSeedReachesTheFaker() {
      var fromDefault = Generate(CreateFactory().CreateSession()).Select(c => c.FirstName).ToList();
      var fromOwnSeed = Generate(CreateFactory(20250921).CreateSession()).Select(c => c.FirstName).ToList();

      fromOwnSeed.ShouldNotBe(fromDefault);
   }

   [Fact]
   public void ALocaleCanBeConfiguredThroughUse() {
      var german = Generate(CreateFactory(5, "de").CreateSession()).Select(c => c.FirstName).ToList();
      var english = Generate(CreateFactory(5, "en").CreateSession()).Select(c => c.FirstName).ToList();

      german.ShouldNotBe(english);
      german.ShouldAllBe(v => !string.IsNullOrWhiteSpace(v));
   }

   /// <summary>
   ///   A source handed in at generation time has to replace the configured one. Asserting that the member
   ///   merely holds something would pass even when the override was silently dropped, because the factory
   ///   already configures that member, so this compares against the value without the override.
   /// </summary>
   [Fact]
   public void ASourceGivenAtGenerationTimeReplacesTheConfiguredOne() {
      const int seed = 4711;

      var withoutOverride = CreateFactory(seed).CreateSession()
         .Single<Customer>()
         .Get()
         .FirstName;

      var withOverride = CreateFactory(seed).CreateSession()
         .Single<Customer>()
         .Source(c => c.FirstName, new FakerFirstNameSource("ja"))
         .Get()
         .FirstName;

      withOverride.ShouldNotBeNullOrWhiteSpace();
      withOverride.ShouldNotBe(withoutOverride);
   }
}
