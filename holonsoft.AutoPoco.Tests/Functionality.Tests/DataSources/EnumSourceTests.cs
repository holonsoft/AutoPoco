using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class EnumSourceTests {
   private enum MyTestEnum {
      one,
      two,
      three,
      four,
      five,
      six,
      seven,
      eight,
      nine,
      ten
   };

   [Fact]
   public void NextReturnsAEnumValue() {
      var source = new EnumSource<MyTestEnum>();
      var value = source.Next(null);

      value.Should().Be(MyTestEnum.three);
   }
}
