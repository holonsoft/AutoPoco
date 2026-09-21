using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class IssnSourceTests : TestBase {
   /// <summary>
   ///   Real periodical numbers, so the validator is measured against the world before it judges the source.
   /// </summary>
   [Theory]
   [InlineData("03785955")]   // 0378-5955, the ISSN example of the standard itself
   [InlineData("20493630")]   // 2049-3630, check character zero
   public void TheValidatorAcceptsRealIssn(string value)
      => IdentifierValidation.IsValidIssn(value).ShouldBeTrue();

   [Theory]
   [InlineData("03785956")]      // check character off by one
   [InlineData("03785055")]      // a data digit off by one
   [InlineData("0378595X")]      // X where a 5 belongs
   [InlineData("X3785955")]      // an X is only allowed in the last position
   [InlineData("0378595")]       // one character short
   [InlineData("037859550")]     // one character too many
   [InlineData("")]
   [InlineData(null)]
   public void TheValidatorRejectsABrokenIssn(string? value)
      => IdentifierValidation.IsValidIssn(value).ShouldBeFalse();

   [Fact]
   public void EveryIssnHasEightCharactersAndAValidCheckCharacter() {
      var source = new IssnSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         value.Length.ShouldBe(8);
         value[..7].ShouldAllBe(c => c >= '0' && c <= '9');
         IdentifierValidation.IsValidIssn(value).ShouldBeTrue($"'{value}' has a broken check character");
      }
   }

   /// <summary>
   ///   The check character ten is written as an X and comes up in roughly one of eleven numbers, so a sample
   ///   of a few hundred has to contain one, otherwise the mod 11 branch is dead code.
   /// </summary>
   [Fact]
   public void ACheckCharacterOfTenIsWrittenAsAnX() {
      var source = new IssnSource();
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v[v.Length - 1] == 'X');
      values.ShouldAllBe(v => IdentifierValidation.IsValidIssn(v));
   }

   [Fact]
   public void EveryCheckCharacterOccurs() {
      var source = new IssnSource();

      Enumerable.Range(0, 3000)
         .Select(_ => source.Next(null)[^1])
         .Distinct()
         .OrderBy(c => c)
         .ShouldBe("0123456789X".ToCharArray());
   }

   [Fact]
   public void NoSingleCharacterChangeOfAGeneratedNumberStillValidates() {
      var source = new IssnSource();

      for (var i = 0; i < 20; i++) {
         var value = source.Next(null);

         foreach (var mutated in IdentifierValidation.SingleCharacterMutations(value, true))
            IdentifierValidation.IsValidIssn(mutated).ShouldBeFalse($"'{mutated}' passed although '{value}' was the valid number");
      }
   }

   [Fact]
   public void AnIssnCarriesNoHyphen()
      => Enumerable.Range(0, 100).Select(_ => new IssnSource().Next(null)).ShouldAllBe(v => !v.Contains('-'));

   [Fact]
   public void NextReturnsStableIssnListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new IssnSource(), new string[] { "3431380X", "72261803", "96364327", "28650034", "6550609X", "2255243X", "89148320", "97669172", "43067107", "00518638" });

   [Fact]
   public void NextReturnsStableIssnListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableIssnSource()!, new string?[] { "3431380X", "72261803", "96364327", "28650034", "6550609X", "2255243X", "89148320", "97669172", "43067107", "00518638", "65767039", "41380304", "17124387", "30731143", "03026000", "80791581", "15904679", "97931276", "42386721", "0480757X", "03556093", "63976781", "5002938X", "97940070", null, "81761805", "72797967", null, "86097709", "19930135" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidNumbers() {
      var source = new NullableIssnSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IdentifierValidation.IsValidIssn(value).ShouldBeTrue($"'{value}' has a broken check character");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new IssnSource().Next(null)).ShouldAllBe(v => v != null);
}
