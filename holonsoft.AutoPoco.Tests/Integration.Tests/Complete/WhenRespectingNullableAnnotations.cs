using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenRespectingNullableAnnotations {
   private static IGenerationSessionFactory CreateFactory(int? threshold, bool respectNullableAnnotations = true)
      => AutoPocoContainer.Configure(x => {
         if (respectNullableAnnotations)
            x.RespectNullableAnnotations(threshold);

         x.Include<NullableMembersClass>()
            .Setup(c => c.NullableText).Use<FirstNameSource>()
            .Setup(c => c.NonNullableText).Use<FirstNameSource>()
            .Setup(c => c.NullableNumber).From(() => 42)
            .Setup(c => c.NullableDate).From(() => DateTime.UnixEpoch)
            .Setup(c => c.NullableField).Use<LastNameSource>()
            .Setup(c => c.NonNullableField).Use<LastNameSource>();
      });

   private static List<NullableMembersClass> Generate(IGenerationSession session, int count)
      => session.Collection<NullableMembersClass>(count).ToList();

   [Fact]
   public void WithoutOptInNullableMembersAreNeverNull() {
      var items = Generate(CreateFactory(100, respectNullableAnnotations: false).CreateSession(), 200);

      items.ShouldAllBe(x => x.NullableText != null);
      items.ShouldAllBe(x => x.NullableNumber != null);
      items.ShouldAllBe(x => x.NullableDate != null);
      items.ShouldAllBe(x => x.NullableField != null);
   }

   [Fact]
   public void WithTheDefaultThresholdNullableMembersAreSometimesNullAndNonNullableMembersNever() {
      var items = Generate(CreateFactory(null).CreateSession(), 300);

      items.ShouldContain(x => x.NullableText == null);
      items.ShouldContain(x => x.NullableText != null);
      items.ShouldContain(x => x.NullableNumber == null);
      items.ShouldContain(x => x.NullableNumber == 42);
      items.ShouldContain(x => x.NullableDate == null);
      items.ShouldContain(x => x.NullableDate == DateTime.UnixEpoch);
      items.ShouldContain(x => x.NullableField == null);
      items.ShouldContain(x => x.NullableField != null);

      items.ShouldAllBe(x => x.NonNullableText != null);
      items.ShouldAllBe(x => x.NonNullableField != null);
   }

   [Fact]
   public void WithAThresholdOf100EveryNullableMemberIsNull() {
      var items = Generate(CreateFactory(100).CreateSession(), 50);

      items.ShouldAllBe(x => x.NullableText == null);
      items.ShouldAllBe(x => x.NullableNumber == null);
      items.ShouldAllBe(x => x.NullableDate == null);
      items.ShouldAllBe(x => x.NullableField == null);

      items.ShouldAllBe(x => x.NonNullableText != null && x.NonNullableText != "");
      items.ShouldAllBe(x => x.NonNullableField != null && x.NonNullableField != "");
   }

   [Fact]
   public void WithAThresholdOf0NoNullableMemberIsNull() {
      var items = Generate(CreateFactory(0).CreateSession(), 200);

      items.ShouldAllBe(x => x.NullableText != null);
      items.ShouldAllBe(x => x.NullableNumber == 42);
      items.ShouldAllBe(x => x.NullableDate == DateTime.UnixEpoch);
      items.ShouldAllBe(x => x.NullableField != null);
   }

   [Fact]
   public void ImposeIsNeverTouched() {
      var session = CreateFactory(100).CreateSession();

      var item = session.Next<NullableMembersClass>(g => g.Impose(x => x.NullableText, "fixed").Impose(x => x.NullableNumber, 7));

      item.NullableText.ShouldBe("fixed");
      item.NullableNumber.ShouldBe(7);
   }

   [Fact]
   public void GenerationTimeSourceOverridesAreWrappedToo() {
      var full = CreateFactory(100).CreateSession();
      var none = CreateFactory(0).CreateSession();

      full.Single<NullableMembersClass>().Source(x => x.NullableText, new FirstNameSource()).Get().NullableText.ShouldBeNull();
      full.Single<NullableMembersClass>().Source(x => x.NullableText, () => "lambda").Get().NullableText.ShouldBeNull();
      none.Single<NullableMembersClass>().Source(x => x.NullableText, new FirstNameSource()).Get().NullableText.ShouldNotBeNull();
      none.Single<NullableMembersClass>().Source(x => x.NullableText, () => "lambda").Get().NullableText.ShouldBe("lambda");
   }

   [Fact]
   public void GenerationTimeSourceOverridesOnNonNullableMembersAreLeftAlone() {
      var session = CreateFactory(100).CreateSession();

      session.Single<NullableMembersClass>().Source(x => x.NonNullableText, () => "kept").Get().NonNullableText.ShouldBe("kept");
   }

   [Fact]
   public void CollectionSourceOverridesAreWrappedToo() {
      var session = CreateFactory(100).CreateSession();

      var items = session.List<NullableMembersClass>(20).Source(x => x.NullableText, new FirstNameSource()).Get();

      items.ShouldAllBe(x => x.NullableText == null);
   }

   [Fact]
   public void NullableMembersDoNotBecomeNullTogether() {
      var items = Generate(CreateFactory(50).CreateSession(), 300);

      items.ShouldContain(x => x.NullableText == null && x.NullableNumber != null);
      items.ShouldContain(x => x.NullableText != null && x.NullableNumber == null);
      items.ShouldContain(x => x.NullableText == null && x.NullableField != null);
   }

   [Fact]
   public void TwoSessionsOfTheSameFactoryProduceTheSameNullPattern() {
      var factory = CreateFactory(50);

      var first = Generate(factory.CreateSession(), 100).Select(x => (x.NullableText == null, x.NullableNumber == null, x.NullableField == null)).ToList();
      var second = Generate(factory.CreateSession(), 100).Select(x => (x.NullableText == null, x.NullableNumber == null, x.NullableField == null)).ToList();

      first.ShouldBe(second);
      first.ShouldContain(x => x.Item1);
      first.ShouldContain(x => !x.Item1);
   }

   [Fact]
   public void SourcesProvidedByConventionsAreWrappedToo() {
      var session = AutoPocoContainer.Configure(x => {
         x.RespectNullableAnnotations(100);
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<NullableMembersClass>();
      }).CreateSession();

      var items = Generate(session, 20);

      // the string convention would deliver "" for both, the annotation turns the nullable one into null
      items.ShouldAllBe(x => x.NullableText == null);
      items.ShouldAllBe(x => x.NonNullableText == "");
      items.ShouldAllBe(x => x.NullableReference == null);
   }

   [Fact]
   public void ObliviousReferenceMembersAreLeftAloneButNullableValueTypesAreNot() {
      var session = AutoPocoContainer.Configure(x => {
         x.RespectNullableAnnotations(100);
         x.Include<ObliviousMembersClass>()
            .Setup(c => c.Text).Use<FirstNameSource>()
            .Setup(c => c.Number).From(() => (int?) 1);
      }).CreateSession();

      var items = session.Collection<ObliviousMembersClass>(20).ToList();

      items.ShouldAllBe(x => x.Text != null);
      items.ShouldAllBe(x => x.Number == null);
   }

   [Fact]
   public void NullableSourcesKeepProducingTheirOwnNulls() {
      // a source that produces nulls on its own is not "un-nulled" by a threshold of 0
      var session = AutoPocoContainer.Configure(x => {
         x.RespectNullableAnnotations(0);
         x.Include<NullableMembersClass>().Setup(c => c.NullableText).Use<NullableFirstNameSource>(100);
      }).CreateSession();

      Generate(session, 20).ShouldAllBe(x => x.NullableText == null);
   }
}
