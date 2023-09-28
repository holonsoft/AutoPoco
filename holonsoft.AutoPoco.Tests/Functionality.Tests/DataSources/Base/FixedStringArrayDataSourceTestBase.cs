using FluentAssertions;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public abstract class FixedStringArrayDataSourceTest<T>()
   : FixedStringArrayDataSourceTestBase<T>
   where T : FixedStringArraySourceBase, new() { }

public abstract class FixedStringArrayDataSourceTestBase<T>
   where T : DataSourceBase<string>, new() {

   protected static void PerformTest(params string[] expectedValues) {
      var source = new T();

      List<string> generated = new();

      for (var i = 0; i < 10; i++)
         generated.Add(source.Next(null));

      generated.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(expectedValues);

      List<string> randomGenerated1 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated1.Add(source.Next(null));

      randomGenerated1.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And.ContainItemsAssignableTo<string>();

      List<string> randomGenerated2 = new();
      source.SetSeedToRandomValue();
      for (var i = 0; i < 10; i++)
         randomGenerated2.Add(source.Next(null));

      randomGenerated2.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And.ContainItemsAssignableTo<string>();

      randomGenerated1.Should().NotBeEquivalentTo(generated);
      randomGenerated2.Should().NotBeEquivalentTo(generated);
      randomGenerated1.Should().NotBeEquivalentTo(randomGenerated2);
   }
}