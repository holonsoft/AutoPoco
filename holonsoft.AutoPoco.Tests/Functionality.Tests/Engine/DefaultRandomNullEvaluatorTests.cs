using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class DefaultRandomNullEvaluatorTests {
   private static List<bool> Draw(DefaultRandomNullEvaluator evaluator, int count)
      => Enumerable.Range(0, count).Select(_ => evaluator.ShouldNextValueReturnNull()).ToList();

   [Fact]
   public void DefaultThresholdComesFromGlobalSettings()
      => new DefaultRandomNullEvaluator().ThresholdPercentage.ShouldBe(AutoPocoGlobalSettings.NullCreationThreshold);

   [Fact]
   public void ExplicitThresholdIsKept()
      => new DefaultRandomNullEvaluator(42).ThresholdPercentage.ShouldBe(42);

   [Fact]
   public void ZeroThresholdNeverReturnsNull()
      => Draw(new DefaultRandomNullEvaluator(0), 1000).ShouldAllBe(x => x == false);

   [Fact]
   public void FullThresholdAlwaysReturnsNull()
      => Draw(new DefaultRandomNullEvaluator(100), 1000).ShouldAllBe(x => x == true);

   [Fact]
   public void MediumThresholdReturnsBothOutcomes() {
      var draws = Draw(new DefaultRandomNullEvaluator(50), 200);
      draws.ShouldContain(true);
      draws.ShouldContain(false);
   }

   [Fact]
   public void SameSeedProducesTheSameSequence() {
      var first = new DefaultRandomNullEvaluator(50);
      var second = new DefaultRandomNullEvaluator(50);
      first.SetSeedToRandomValue(2024);
      second.SetSeedToRandomValue(2024);

      Draw(first, 200).ShouldBe(Draw(second, 200));
   }

   [Fact]
   public void DifferentSeedsProduceDifferentSequences() {
      var first = new DefaultRandomNullEvaluator(50);
      var second = new DefaultRandomNullEvaluator(50);
      first.SetSeedToRandomValue(1);
      second.SetSeedToRandomValue(2);

      Draw(first, 200).ShouldNotBe(Draw(second, 200));
   }

   [Fact]
   public void TwoDefaultInstancesAreRepeatable()
      => Draw(new DefaultRandomNullEvaluator(50), 200).ShouldBe(Draw(new DefaultRandomNullEvaluator(50), 200));
}
