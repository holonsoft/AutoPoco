using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class RandomNumberGeneratorTests {
   [Fact]
   public void CurrentIsASingleton()
      => RandomNumberGenerator.Current.ShouldBeSameAs(RandomNumberGenerator.Current);

   [Fact]
   public void NextWithMaxStaysBelowMax() {
      var generator = new RandomNumberGenerator();
      for (var i = 0; i < 500; i++)
         generator.Next(10).ShouldBeInRange(0, 9);
   }

   [Fact]
   public void NextWithMinAndMaxStaysInRange() {
      var generator = new RandomNumberGenerator();
      for (var i = 0; i < 500; i++)
         generator.Next(5, 10).ShouldBeInRange(5, 9);
   }

   [Fact]
   public void NextWithoutBoundsIsNonNegative() {
      var generator = new RandomNumberGenerator();
      for (var i = 0; i < 500; i++)
         generator.Next().ShouldBeGreaterThanOrEqualTo(0);
   }

   [Fact]
   public void NextDoubleIsInUnitInterval() {
      var generator = new RandomNumberGenerator();
      for (var i = 0; i < 500; i++)
         generator.NextDouble().ShouldBeInRange(0.0, 0.999999999999);
   }

   [Fact]
   public void NextBytesFillsTheBuffer() {
      var generator = new RandomNumberGenerator();
      var buffer = new byte[64];

      generator.NextBytes(buffer);

      buffer.ShouldContain(b => b != 0);
   }

   [Fact]
   public void TwoInstancesWithDefaultSeedAreRepeatable() {
      var first = new RandomNumberGenerator();
      var second = new RandomNumberGenerator();

      Enumerable.Range(0, 20).Select(_ => first.Next()).ToList()
         .ShouldBe(Enumerable.Range(0, 20).Select(_ => second.Next()).ToList());
   }

   [Fact]
   public void ExplicitSeedIsRepeatable() {
      var first = new RandomNumberGenerator();
      var second = new RandomNumberGenerator();
      first.SetSeedToRandomValue(12345);
      second.SetSeedToRandomValue(12345);

      Enumerable.Range(0, 20).Select(_ => first.Next(1000)).ToList()
         .ShouldBe(Enumerable.Range(0, 20).Select(_ => second.Next(1000)).ToList());
   }

   [Fact]
   public void ActsAsAnIntDataSource() {
      IDataSource source = new RandomNumberGenerator();
      source.InternalNext(null).ShouldBeOfType<int>();
   }
}
