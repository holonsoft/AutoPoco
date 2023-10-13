using FluentAssertions;
using holonsoft.AutoPoco.Engine;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class TestBase() {
   public static void NextReturnsStableElementListInTermsOfTestability<T>(DataSourceBase<T> source, params T[] expectedValues) {
      List<T> generated = new();

      for (var i = 0; i < expectedValues.Length; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .HaveCount(expectedValues.Length)
         .And
         .ContainInOrder(expectedValues)
         .And.ContainItemsAssignableTo<T>();

      if (typeof(T) == typeof(bool) || (typeof(T) == typeof(bool?)))
         return;

      List<T> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .HaveCount(10);

      List<T> randomGenerated2 = new();
      source.SetSeedToRandomValue(1968);
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Should()
         .HaveCount(10);

      randomGenerated1.Should().NotBeEquivalentTo(expectedValues);
      randomGenerated2.Should().NotBeEquivalentTo(expectedValues);
      randomGenerated1.Should().NotBeEquivalentTo(randomGenerated2);

   }
}
