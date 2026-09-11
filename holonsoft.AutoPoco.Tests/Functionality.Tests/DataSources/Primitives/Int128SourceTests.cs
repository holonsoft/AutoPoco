using Shouldly;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class Int128SourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new Int128Source();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new Int128[] { Int128.Parse("52574408570445668602952862031623518323"), Int128.Parse("-81454287396521681934135293700379577423"), Int128.Parse("108627274490468967281591080578071069176"), Int128.Parse("-1701883700024824354554067136260673513"), Int128.Parse("-107321079751597364356135199189103805246"), Int128.Parse("165769995339853085516163578607075436844"), Int128.Parse("129337432268046073300617073194816895354"), Int128.Parse("-34365565662198526319518735958392579928"), Int128.Parse("-82604512145917092532540618957667971715"), Int128.Parse("146716362576520372719327904926591472454") };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableInt128Source();
      var expectedValues = new Int128?[] { Int128.Parse("107009837290989317134782020631440535260"), Int128.Parse("146617463808392992256105003233854242371"), Int128.Parse("52574408570445668602952862031623518323"), Int128.Parse("-81454287396521681934135293700379577423"), Int128.Parse("108627274490468967281591080578071069176"), Int128.Parse("-1701883700024824354554067136260673513"), Int128.Parse("-107321079751597364356135199189103805246"), Int128.Parse("165769995339853085516163578607075436844"), Int128.Parse("129337432268046073300617073194816895354"), Int128.Parse("-34365565662198526319518735958392579928") };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextCoversTheWholeRangeOfTheTypeIncludingNegativeValues() {
      var values = Draw(new Int128Source(), 200);

      values.ShouldContain(x => x < Int128.Zero);
      values.ShouldContain(x => x > Int128.Zero);
   }

   [Fact]
   public void NextReachesBothEndsOfAShortRange() {
      var source = new Int128Source(Int128.MaxValue - 2, Int128.MaxValue);
      var values = Draw(source, 300);

      values.ShouldAllBe(x => x >= Int128.MaxValue - 2 && x <= Int128.MaxValue);
      values.ShouldContain(Int128.MaxValue - 2);
      values.ShouldContain(Int128.MaxValue);
   }

   [Fact]
   public void NextStaysInARestrictedRangeInsteadOfPilingUpOnTheBounds() {
      var source = new Int128Source(1, 5);
      var values = Draw(source, 500);

      values.ShouldAllBe(x => x >= 1 && x <= 5);
      values.Distinct().Count().ShouldBe(5);
   }

   [Fact]
   public void NextWithMinEqualToMaxReturnsThatValue() {
      var source = new Int128Source(42, 42);

      Draw(source, 10).ShouldAllBe(x => x == 42);
   }

   [Fact]
   public void NextThrowsWhenMaxIsBelowMin() {
      var source = new Int128Source(10, 5);

      Should.Throw<ArgumentOutOfRangeException>(() => source.Next(null));
   }

   private static List<Int128> Draw(Int128Source source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();
}