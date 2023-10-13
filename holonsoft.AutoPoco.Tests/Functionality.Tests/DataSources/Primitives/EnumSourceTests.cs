using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class EnumSourceTests : TestBase {
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

   [Fact]
   public void NextReturnsStableDateTimeListInTermsOfTestability() {
      var source = new EnumSource<MyTestEnum>();
      NextReturnsStableElementListInTermsOfTestability(source, new MyTestEnum[] {
         MyTestEnum.three, MyTestEnum.two, MyTestEnum.four, MyTestEnum.ten,
      });
   }

   //[Fact]
   //public void NextReturnsStableDateTimeListInTermsOfTestabilityAndListCanContainNull() {
   //   var source = new NullableEnumSource<MyTestEnum?>();
   //   NextReturnsStableElementListInTermsOfTestability<MyTestEnum?>(source, new MyTestEnum?[] {
   //      MyTestEnum.three, MyTestEnum.two, MyTestEnum.four, MyTestEnum.ten, null
   //   });
   //}
}
