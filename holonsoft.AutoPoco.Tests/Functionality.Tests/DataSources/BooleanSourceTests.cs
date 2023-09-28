using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class BooleanSourceTests {
   [Fact]
   public void NextReturnsABoolean() {
      var source = new BooleanSource();

      var generated = new bool[10];

      for (var i = 0; i < 10; i++)
         generated[i] = source.Next(null);

      var collection = generated as ICollection<bool>;

      collection.Should()
         .NotBeEmpty()
         .And
         .HaveCount(10)
         .And
         .ContainInOrder(new[] { false, false, false, true, true, true, true, false, false, false })
         .And.ContainItemsAssignableTo<bool>();
   }
}
