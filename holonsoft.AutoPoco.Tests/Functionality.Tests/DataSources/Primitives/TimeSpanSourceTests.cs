using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class TimeSpanSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestability() {
      var source = new TimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));
      var value1 = source.Next(null);
      var value2 = source.Next(null);

      value2.Should().NotBe(value1);

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan[] {
            new(-3700680842),
            new(8627187683),
            new(6849344408),
            new(9205046584),
            new(9156634835),
            new(-6115511263),
            new(-2264435866),
            new(-4121559893),
            new(-3987557150),
            new(-4349441889),
         });
   }

   [Fact]
   public void NextReturnsStableTimeSpanListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableTimeSpanSource(new TimeSpan(-10000000000), new TimeSpan(10000000000));

      NextReturnsStableElementListInTermsOfTestability(source, new TimeSpan?[] {
         new(-5822233184),
         null,
         new(-7639658450),
         new(-3700680842),
         new(8627187683),
         new(6849344408),
         new(9205046584),
         new(9156634835),
         new(-6115511263),
         new(-2264435866),
         });
   }
}