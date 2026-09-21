using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class IsbnSourceTests : TestBase {
   /// <summary>
   ///   Real book numbers, so the validator is measured against the world before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("0306406152")]   // The C Programming Language, the classic mod 11 example
   [InlineData("0198526636")]
   [InlineData("080442957X")]   // check digit ten, written as an X
   public void TheValidatorAcceptsRealIsbn10(string value)
      => IdentifierValidation.IsValidIsbn10(value).ShouldBeTrue();

   [Theory]
   [InlineData("0306406153")]   // check digit off by one
   [InlineData("0306406252")]   // a data digit off by one
   [InlineData("030640615X")]   // X where a 2 belongs
   [InlineData("X306406152")]   // an X is only allowed in the last position
   [InlineData("030640615")]    // one character short
   [InlineData("9783161484100")]
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsBrokenIsbn10(string? value)
      => IdentifierValidation.IsValidIsbn10(value).ShouldBeFalse();

   [Fact]
   public void TheDefaultFormatIsIsbn13() {
      var value = new IsbnSource().Next(null);
      value.Length.ShouldBe(13);
      IdentifierValidation.IsValidGtin(value).ShouldBeTrue();
   }

   [Fact]
   public void EveryIsbn13IsAValidGtin13WithABookPrefix() {
      var source = new IsbnSource(IsbnFormat.Isbn13);

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(13);
         value.ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   /// <summary>
   ///   Only registration groups that ISBN International actually hands out. 978 carries the single digit
   ///   groups 0 to 5 and 7, 979 carries 8 and the two digit groups 10, 11 and 12.
   /// </summary>
   private static readonly string[] _allocatedGroupPrefixes =
      ["9780", "9781", "9782", "9783", "9784", "9785", "9787", "9798", "97910", "97911", "97912"];

   private static bool IsInAnAllocatedGroup(string value)
      => _allocatedGroupPrefixes.Any(p => value.StartsWith(p, StringComparison.Ordinal));

   /// <summary>
   ///   The list above is a copy of the one inside the source, so it needs an anchor outside of both. These
   ///   are published ISBN-13s, their registration group has to be one the list knows.
   /// </summary>
   [Theory]
   [InlineData("9780306406157")]   // The C Programming Language, group 978-0
   [InlineData("9783161484100")]   // group 978-3
   public void ThePublishedGroupsAreKnownToTheList(string realIsbn)
      => IsInAnAllocatedGroup(realIsbn).ShouldBeTrue();

   /// <summary>
   ///   The other half of the anchor: prefixes that are no ISBN registration group at all, or that open a
   ///   multi digit group this source stays out of. None of them may ever come out of the source.
   ///   979-0 is the ISMN range of sheet music, the source used to draw the group digit freely and produced
   ///   exactly such numbers.
   /// </summary>
   [Theory]
   [InlineData("9786")]   // 978-6 is no single digit group, the 6xx space holds three digit groups
   [InlineData("9788")]   // opens the two digit groups 80 to 94
   [InlineData("9789")]   // opens the three and five digit groups
   [InlineData("9790")]   // ISMN, sheet music
   [InlineData("9792")]
   [InlineData("9793")]
   [InlineData("9794")]
   [InlineData("9795")]
   [InlineData("9796")]
   [InlineData("9797")]
   [InlineData("9799")]
   public void AGroupThatIsNotAllocatedIsNeverProduced(string forbiddenPrefix) {
      _allocatedGroupPrefixes.ShouldNotContain(forbiddenPrefix);

      var source = new IsbnSource(IsbnFormat.Isbn13);
      Enumerable.Range(0, 1000)
         .Select(_ => source.Next(null))
         .ShouldAllBe(v => !v.StartsWith(forbiddenPrefix, StringComparison.Ordinal));
   }

   [Fact]
   public void EveryIsbn13StartsWithAnAllocatedRegistrationGroup() {
      var source = new IsbnSource(IsbnFormat.Isbn13);

      for (var i = 0; i < 1000; i++) {
         var value = source.Next(null);
         IsInAnAllocatedGroup(value).ShouldBeTrue($"'{value}' is not in an allocated ISBN registration group");
      }
   }

   [Fact]
   public void EveryAllocatedRegistrationGroupOccurs() {
      var source = new IsbnSource(IsbnFormat.Isbn13);
      var values = Enumerable.Range(0, 2000).Select(_ => source.Next(null)).ToList();
      var missing = _allocatedGroupPrefixes
         .Where(p => !values.Any(v => v.StartsWith(p, StringComparison.Ordinal)))
         .ToList();

      missing.ShouldBeEmpty($"never produced the groups {string.Join(", ", missing)}");
   }

   /// <summary>
   ///   A 979 group is five characters wide, a 978 group four, so the loop that fills the rest of the number
   ///   starts at a different index. The wider branch has to reach the full length as well.
   /// </summary>
   [Fact]
   public void TheFiveCharacterGroupsProduceFullLengthNumbers() {
      var source = new IsbnSource(IsbnFormat.Isbn13);

      var wideGroups = Enumerable.Range(0, 1000)
         .Select(_ => source.Next(null))
         .Where(v => v.StartsWith("9791", StringComparison.Ordinal))
         .ToList();

      wideGroups.ShouldNotBeEmpty();

      foreach (var value in wideGroups) {
         value.Length.ShouldBe(13);
         value.ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   [Fact]
   public void EveryIsbn10HasTenCharactersAndAValidCheckDigit() {
      var source = new IsbnSource(IsbnFormat.Isbn10);

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(10);
         value[..9].ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidIsbn10(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }

   /// <summary>
   ///   The check digit ten is written as an X. It comes up in roughly one of eleven numbers, so a sample
   ///   of a few hundred has to contain one, otherwise the mod 11 branch is dead code.
   /// </summary>
   [Fact]
   public void AnIsbn10CheckDigitOfTenIsWrittenAsAnX() {
      var source = new IsbnSource(IsbnFormat.Isbn10);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v[v.Length - 1] == 'X');
      values.ShouldAllBe(v => IdentifierValidation.IsValidIsbn10(v));
   }

   /// <summary>
   ///   Every check digit from 0 to 9 and the X have to be reachable.
   /// </summary>
   [Fact]
   public void EveryIsbn10CheckDigitOccurs() {
      var source = new IsbnSource(IsbnFormat.Isbn10);

      var seen = Enumerable.Range(0, 3000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ToList();

      seen.ShouldBe("0123456789X".ToCharArray());
   }

   [Fact]
   public void NoSingleCharacterChangeOfAGeneratedIsbn13StillValidates() {
      var source = new IsbnSource(IsbnFormat.Isbn13);

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, false))
            IdentifierValidation.IsValidGtin(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   [Fact]
   public void NoSingleCharacterChangeOfAGeneratedIsbn10StillValidates() {
      var source = new IsbnSource(IsbnFormat.Isbn10);

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, true))
            IdentifierValidation.IsValidIsbn10(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   [Fact]
   public void AnIsbnCarriesNoHyphens() {
      var source = new IsbnSource(IsbnFormat.Isbn13);
      Enumerable.Range(0, 100).Select(_ => source.Next(null)).ShouldAllBe(v => !v.Contains('-'));
   }

   [Fact]
   public void AnUnknownFormatIsRejected() {
      var ex = Should.Throw<ArgumentOutOfRangeException>(() => new IsbnSource((IsbnFormat) 12));
      ex.ParamName.ShouldBe("format");
   }

   [Fact]
   public void NextReturnsStableIsbn13ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new IsbnSource(), new string[] { "9783431380729", "9782618096361", "9784322865004", "9783655060926", "9782552438913", "9784832976696", "9781743067109", "9780051863656", "9798670341387", "9780301712437" });

   [Fact]
   public void NextReturnsStableIsbn10ListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new IsbnSource(IsbnFormat.Isbn10), new string[] { "3431380727", "2618096363", "4322865003", "3655060920", "2552438911", "4832976699", "1743067100", "0051863650", "7670341386", "0301712433" });

   [Fact]
   public void NextReturnsStableIsbnListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableIsbnSource()!, new string?[] { "9783431380729", "9782618096361", "9784322865004", "9783655060926", "9782552438913", "9784832976696", "9781743067109", "9780051863656", "9798670341387", "9780301712437", "9791030731149", "9780302600801", "9798915815901", "9784679793128", "9798423867201", "9784807570355", "9785609639769", "9798850029388", "9791179400784", "9781761807275", "9791179686096", "9798701993011", "9783062832864", "9798899735851", null, "9784213859198", "9781312451780", null, "9798313441153", "9798778656253" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableIsbnSource(IsbnFormat.Isbn10, 50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidIsbn10(value).ShouldBeTrue($"'{value}' has a broken check digit");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new IsbnSource().Next(null)).ShouldAllBe(v => v != null);

   /// <summary>
   ///   Every constructor overload has to end up at the same behaviour, a valid number of the asked for form.
   /// </summary>
   [Fact]
   public void EveryNullableOverloadProducesValidNumbers() {
      AllNonNullValuesAreValid(new NullableIsbnSource(), IsbnFormat.Isbn13);
      AllNonNullValuesAreValid(new NullableIsbnSource(IsbnFormat.Isbn10), IsbnFormat.Isbn10);
      AllNonNullValuesAreValid(new NullableIsbnSource(50), IsbnFormat.Isbn13);
      AllNonNullValuesAreValid(new NullableIsbnSource(IsbnFormat.Isbn10, 50), IsbnFormat.Isbn10);
   }

   private static void AllNonNullValuesAreValid(IsbnSourceBase source, IsbnFormat expectedFormat) {
      var values = Enumerable.Range(0, 200).Select(_ => source.Next(null)).Where(v => v is not null).ToList();

      values.ShouldNotBeEmpty();

      foreach (var value in values) {
         value.Length.ShouldBe((int) expectedFormat);

         if (expectedFormat == IsbnFormat.Isbn13)
            IdentifierValidation.IsValidGtin(value).ShouldBeTrue($"'{value}' has a broken check digit");
         else
            IdentifierValidation.IsValidIsbn10(value).ShouldBeTrue($"'{value}' has a broken check digit");
      }
   }
}
