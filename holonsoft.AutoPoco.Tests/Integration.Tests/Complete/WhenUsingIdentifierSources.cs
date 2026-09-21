using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

/// <summary>
///   An article as a shared test database would hold it: the barcode the scanner reads, the number the
///   book trade uses and the number the marketplace assigns.
/// </summary>
public class ArticleWithIdentifiers {
   public string Ean { get; set; } = "";
   public string CartonGtin { get; set; } = "";
   public string Isbn { get; set; } = "";
   public string Asin { get; set; } = "";
}

public class WhenUsingIdentifierSources {
   private static IGenerationSessionFactory CreateFactory(int? seed = null)
      => AutoPocoContainer.Configure(x => {
         if (seed.HasValue)
            x.UseSeed(seed.Value);

         x.Include<ArticleWithIdentifiers>()
            .Setup(a => a.Ean).Use<Ean13Source>()
            .Setup(a => a.CartonGtin).Use<GtinSource>(GtinFormat.Gtin14)
            .Setup(a => a.Isbn).Use<IsbnSource>(IsbnFormat.Isbn10)
            .Setup(a => a.Asin).Use<AsinSource>();
      });

   private static List<ArticleWithIdentifiers> Generate(IGenerationSession session, int count = 50)
      => session.Collection<ArticleWithIdentifiers>(count).ToList();

   [Fact]
   public void EveryGeneratedArticleCarriesValidIdentifiers() {
      foreach (var article in Generate(CreateFactory().CreateSession())) {
         IdentifierValidation.IsValidGtin(article.Ean).ShouldBeTrue($"'{article.Ean}' is not a valid EAN-13");
         article.Ean.Length.ShouldBe(13);

         IdentifierValidation.IsValidGtin(article.CartonGtin).ShouldBeTrue($"'{article.CartonGtin}' is not a valid GTIN-14");
         article.CartonGtin.Length.ShouldBe(14);

         IdentifierValidation.IsValidIsbn10(article.Isbn).ShouldBeTrue($"'{article.Isbn}' is not a valid ISBN-10");

         article.Asin.Length.ShouldBe(10);
         article.Asin[0].ShouldBe('B');
      }
   }

   [Fact]
   public void TwoFactoriesWithTheSameSeedProduceTheSameIdentifiers() {
      var first = Generate(CreateFactory(4711).CreateSession()).Select(a => a.Ean).ToList();
      var second = Generate(CreateFactory(4711).CreateSession()).Select(a => a.Ean).ToList();

      first.ShouldBe(second);
   }

   [Fact]
   public void ADifferentSeedProducesDifferentIdentifiers() {
      var first = Generate(CreateFactory(1).CreateSession()).Select(a => a.Ean).ToList();
      var second = Generate(CreateFactory(2).CreateSession()).Select(a => a.Ean).ToList();

      first.ShouldNotBe(second);
   }

   [Fact]
   public void TwoSessionsOfTheSameFactoryProduceTheSameIdentifiers() {
      var factory = CreateFactory(99);

      Generate(factory.CreateSession()).Select(a => a.Asin)
         .ShouldBe(Generate(factory.CreateSession()).Select(a => a.Asin));
   }

   /// <summary>
   ///   The EAN of the article and the GTIN of its carton are two members, so they draw from two streams.
   ///   Sharing one stream would tie the carton number to the article number.
   /// </summary>
   [Fact]
   public void TwoGtinMembersOfTheSameObjectAreIndependent() {
      var articles = Generate(CreateFactory().CreateSession());

      articles.Select(a => a.Ean[..8]).ShouldNotBe(articles.Select(a => a.CartonGtin[..8]));
   }

   /// <summary>
   ///   The prefix form from the README, configured through the argument overload of <c>Use</c>.
   /// </summary>
   [Fact]
   public void APrefixCanBeConfiguredThroughUse() {
      var factory = AutoPocoContainer.Configure(x =>
         x.Include<ArticleWithIdentifiers>()
            .Setup(a => a.Ean).Use<Ean13Source>("40063"));

      foreach (var article in Generate(factory.CreateSession())) {
         article.Ean.ShouldStartWith("40063");
         IdentifierValidation.IsValidGtin(article.Ean).ShouldBeTrue();
      }
   }

   /// <summary>
   ///   A source handed in at generation time has to behave like a configured one.
   /// </summary>
   [Fact]
   public void ASourceGivenAtGenerationTimeIsUsed() {
      var session = CreateFactory().CreateSession();

      var article = session.Single<ArticleWithIdentifiers>()
         .Source(a => a.Ean, new GtinSource(GtinFormat.Gtin13, "40063"))
         .Get();

      article.Ean.ShouldStartWith("40063");
      IdentifierValidation.IsValidGtin(article.Ean).ShouldBeTrue();
   }
}
