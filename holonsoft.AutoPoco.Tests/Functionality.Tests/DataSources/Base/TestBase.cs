using Shouldly;
using holonsoft.AutoPoco.Engine;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class TestBase() {
   public static void NextReturnsStableElementListInTermsOfTestability<T>(DataSourceBase<T> source, params T[] expectedValues) {
      List<T> generated = new();

      for (var i = 0; i < expectedValues.Length; i++)
         generated.Add(source.Next(null));

      // same count and same order as the expected values
      generated.ShouldBe(expectedValues);

      if (typeof(T) == typeof(bool) || (typeof(T) == typeof(bool?)))
         return;

      List<T> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < expectedValues.Length; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Count.ShouldBe(expectedValues.Length);

      List<T> randomGenerated2 = new();
      source.SetSeedToRandomValue(1968);
      for (var i = 0; i < expectedValues.Length; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Count.ShouldBe(expectedValues.Length);

      // a different seed must produce different data, regardless of order
      HaveSameItems(randomGenerated1, expectedValues).ShouldBeFalse("random seed reproduced the default sequence");
      HaveSameItems(randomGenerated2, expectedValues).ShouldBeFalse("seed 1968 reproduced the default sequence");
      HaveSameItems(randomGenerated1, randomGenerated2).ShouldBeFalse("two different seeds produced the same data");
   }

   /// <summary>
   ///   True when both sequences contain the same items with the same multiplicity, in any order.
   /// </summary>
   private static bool HaveSameItems<T>(IEnumerable<T> first, IEnumerable<T> second) {
      var counts = new Dictionary<object, int>();
      var nullCount = 0;

      foreach (var item in first) {
         if (item is null) nullCount++;
         else counts[item] = counts.GetValueOrDefault(item) + 1;
      }

      foreach (var item in second) {
         if (item is null) {
            if (--nullCount < 0) return false;
            continue;
         }
         if (!counts.TryGetValue(item, out var remaining) || remaining == 0)
            return false;
         counts[item] = remaining - 1;
      }

      return nullCount == 0 && counts.Values.All(c => c == 0);
   }
}
