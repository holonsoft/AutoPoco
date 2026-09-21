using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public class AsinSourceTests : TestBase {
   private static bool IsAsin(string? value)
      => value is { Length: 10 }
         && value[0] == 'B'
         && value.Skip(1).All(c => c is >= '0' and <= '9' or >= 'A' and <= 'Z');

   [Fact]
   public void EveryAsinIsTenCharactersStartingWithB() {
      var source = new AsinSource();

      for (var i = 0; i < 500; i++)
         IsAsin(source.Next(null)).ShouldBeTrue();
   }

   /// <summary>
   ///   An ASIN has no check digit, so the only thing worth pinning besides the shape is that the whole
   ///   alphabet is reachable. A generator that silently dropped the letters would still look like an ASIN.
   /// </summary>
   [Fact]
   public void TheWholeAlphabetIsReachableInEveryPositionButTheFirst() {
      var source = new AsinSource();
      var values = Enumerable.Range(0, 5000).Select(_ => source.Next(null)).ToList();

      for (var position = 1; position < 10; position++) {
         var seen = values.Select(v => v[position]).ToHashSet();
         var missing = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".Where(c => !seen.Contains(c)).ToList();
         missing.ShouldBeEmpty($"position {position} never produced {string.Join(", ", missing)}");
      }
   }

   [Fact]
   public void TheFirstCharacterIsAlwaysB() {
      var source = new AsinSource();
      Enumerable.Range(0, 500).Select(_ => source.Next(null)[0]).Distinct().ShouldBe(new[] { 'B' });
   }

   /// <summary>
   ///   Two ASINs in a row must not be the same. With 36^9 combinations a collision inside a short sample
   ///   would mean the random stream is not advancing.
   /// </summary>
   [Fact]
   public void ConsecutiveAsinsDiffer() {
      var source = new AsinSource();
      var values = Enumerable.Range(0, 1000).Select(_ => source.Next(null)).ToList();
      values.Distinct().Count().ShouldBe(values.Count);
   }

   [Fact]
   public void NextReturnsStableAsinListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new AsinSource(), new string[] { "BS3KJWNS2Y", "BH63RKZQQQ", "BI8U5WG3T6", "B5LAWTMG9V", "BIY5L4ZVP1", "BC4S3AP6RM", "B9HDQR3G6Q", "BN1WGXVSM3", "BVCLQ7WJ4X", "B3RRJFGCEH" });

   [Fact]
   public void NextReturnsStableAsinListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableAsinSource()!, new string?[] { "BS3KJWNS2Y", "BH63RKZQQQ", "BI8U5WG3T6", "B5LAWTMG9V", "BIY5L4ZVP1", "BC4S3AP6RM", "B9HDQR3G6Q", "BN1WGXVSM3", "BVCLQ7WJ4X", "B3RRJFGCEH", "BI4U3V3SJ4", "BSUGDGWO0F", "BP1L8CXSWB", "BQ679NTJ14", "BYJM7A2W4V", "BB0CN5WVJ5", "BDM0NN8LWS", "B3PNKGGHNC", "BNANPMU0BV", "BNHPCZ0XSD", "BJGDCB2STB", "BYF83L4YFJ", "B5PXVXZT1I", "BK5S1TO7US", null, "BF3134VUE1", "B7F56IEZYS", null, "BREZH2U9B0", "BGJ7SQGU5F" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseWellFormedAsins() {
      var source = new NullableAsinSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         IsAsin(value).ShouldBeTrue($"'{value}' is not a well formed ASIN");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new AsinSource().Next(null)).ShouldAllBe(v => v != null);
}
