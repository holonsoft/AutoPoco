using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class LongSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestability() {
      var source = new LongSource();
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.ShouldNotBe(value1);

      var expectedValues = new long[] { 5119210995652640323, 7948148639268725140, -5917255539371771789, 2850064399460885869, -7389121618870602831, -4415645767678372525, -5012591893755526664, 5888696349687283552, 7698675207198666775, -92259300244229156 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }

   [Fact]
   public void NextReturnsStableIntegerListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableLongSource();
      var expectedValues = new long?[] { 3245583091863006940, 5801014903410548207, 5119210995652640323, 7948148639268725140, -5917255539371771789, 2850064399460885869, -7389121618870602831, -4415645767678372525, -5012591893755526664, 5888696349687283552 };
      NextReturnsStableElementListInTermsOfTestability(source, expectedValues);
   }
}