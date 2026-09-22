using System.Text.RegularExpressions;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class PatternSourceTests : TestBase {
   [Theory]
   [InlineData("#####", @"^\d{5}$")]
   [InlineData("@@@", "^[A-Z]{3}$")]
   [InlineData("***", "^[0-9A-Z]{3}$")]
   [InlineData("AB-##", @"^AB-\d{2}$")]                  // letters that are no placeholders stay literal
   [InlineData("ORD/####/@@", @"^ORD/\d{4}/[A-Z]{2}$")]
   [InlineData(@"\#-#", @"^#-\d$")]                      // an escaped placeholder stays literal
   [InlineData(@"\\#", @"^\\\d$")]                       // an escaped backslash stays a backslash
   public void EveryValueMatchesItsPattern(string pattern, string expected) {
      var source = new PatternSource(pattern);
      var regex = new Regex(expected);

      for (var i = 0; i < 200; i++) {
         var value = source.Next(null);
         regex.IsMatch(value).ShouldBeTrue($"'{value}' does not match pattern '{pattern}'");
      }
   }

   [Fact]
   public void EveryPlaceholderCharacterOccurs() {
      var source = new PatternSource("#@*");
      var values = Enumerable.Range(0, 2000).Select(_ => source.Next(null)).ToList();

      values.Select(v => v[0]).Distinct().OrderBy(c => c).ShouldBe("0123456789".ToCharArray());
      values.Select(v => v[1]).Distinct().Count().ShouldBe(26);
      values.Select(v => v[2]).Distinct().Count().ShouldBe(36);
   }

   [Theory]
   [InlineData("")]
   [InlineData(null)]
   [InlineData("ab\\")]      // a lone trailing backslash escapes nothing
   public void ABrokenPatternThrows(string? pattern)
      => Should.Throw<ArgumentException>(() => new PatternSource(pattern!));

   [Fact]
   public void ThePatternIsPublished()
      => new PatternSource("@@-##").Pattern.ShouldBe("@@-##");

   [Fact]
   public void NextReturnsStableValueListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new PatternSource("@@@-#####"), new string[] { "PDU-31380", "XCC-61809", "GDO-64322", "IOW-50036", "FVK-06092", "CFM-52438", "ZBM-48329", "XGW-91743", "QGO-71000", "FBY-63657" });

   [Fact]
   public void NextReturnsStableValueListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullablePatternSource("@@@-#####")!, new string?[] { "PDU-31380", "XCC-61809", "GDO-64322", "IOW-50036", "FVK-06092", "CFM-52438", "ZBM-48329", "XGW-91743", "QGO-71000", "FBY-63657", "WHA-34138", "QMT-01712", "EDI-30731", "REM-03026", "NQA-80791", "VIM-15904", "LPG-79793", "LLB-27423", "IWH-20480", "MXF-70355", "NWA-96397", "GXI-50029", "DIZ-79400", "NLX-81761", null, "PIQ-72797", "ZOW-86097", null, "XQR-99301", "NPT-06283" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseMatchingValues() {
      var source = new NullablePatternSource("####", 50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         Regex.IsMatch(value!, @"^\d{4}$").ShouldBeTrue($"'{value}' does not match the pattern");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new PatternSource("##").Next(null)).ShouldAllBe(v => v != null);
}
