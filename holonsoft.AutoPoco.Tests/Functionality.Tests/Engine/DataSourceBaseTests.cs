using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class DataSourceBaseTests {
   private sealed class NullableCountingSource : DataSourceBase<int?> {
      public int Calls { get; private set; }
      protected override int? GetNextValue(IGenerationContext? context) => ++Calls;
   }

   private sealed class ConstantSource : DataSourceBase<int> {
      protected override int GetNextValue(IGenerationContext? context) => 7;
   }

   private sealed class RandomSource : DataSourceBase<int> {
      protected override int GetNextValue(IGenerationContext? context) => Random.Next();
   }

   private sealed class AlwaysNullEvaluator : IRandomNullEvaluator {
      public int SeedSeen { get; private set; }
      public bool ShouldNextValueReturnNull() => true;
      public void SetSeedToRandomValue(int seed) => SeedSeen = seed;
   }

   private static List<T> Take<T>(DataSourceBase<T> source, int count)
      => Enumerable.Range(0, count).Select(_ => source.Next(null)).ToList();

   [Fact]
   public void NullableTypeWithFullThresholdAlwaysReturnsNullWithoutAskingForAValue() {
      var source = new NullableCountingSource();
      source.SetNullCreationThreshold(100);

      Take(source, 50).ShouldAllBe(x => x == null);
      source.Calls.ShouldBe(0);
   }

   [Fact]
   public void NullableTypeWithZeroThresholdNeverReturnsNull() {
      var source = new NullableCountingSource();
      source.SetNullCreationThreshold(0);

      Take(source, 50).ShouldAllBe(x => x != null);
      source.Calls.ShouldBe(50);
   }

   [Fact]
   public void NonNullableTypeIgnoresTheNullEvaluator() {
      var source = new ConstantSource();
      source.SetNullCreationThreshold(100);

      Take(source, 20).ShouldAllBe(x => x == 7);
   }

   [Fact]
   public void SetNullCreationThresholdReplacesTheEvaluatorAndIsFluent() {
      var source = new NullableCountingSource();
      var before = source.RandomNullEvaluator;

      var result = source.SetNullCreationThreshold(33);

      result.ShouldBeSameAs(source);
      source.RandomNullEvaluator.ShouldNotBeSameAs(before);
      source.RandomNullEvaluator.ShouldBeOfType<DefaultRandomNullEvaluator>()
         .ThresholdPercentage.ShouldBe(33);
   }

   [Fact]
   public void CustomEvaluatorIsUsed() {
      var source = new NullableCountingSource();
      var evaluator = new AlwaysNullEvaluator();
      source.SetRandomNullEvaluator(evaluator);

      source.RandomNullEvaluator.ShouldBeSameAs(evaluator);
      source.Next(null).ShouldBeNull();
   }

   [Fact]
   public void SeedingForwardsTheSeedToTheEvaluator() {
      var source = new NullableCountingSource();
      var evaluator = new AlwaysNullEvaluator();
      source.SetRandomNullEvaluator(evaluator);

      source.SetSeedToRandomValue(4711);

      evaluator.SeedSeen.ShouldBe(4711);
   }

   [Fact]
   public void TwoSourcesWithTheDefaultSeedProduceTheSameSequence()
      => Take(new RandomSource(), 20).ShouldBe(Take(new RandomSource(), 20));

   [Fact]
   public void SameExplicitSeedProducesTheSameSequence() {
      var first = new RandomSource();
      var second = new RandomSource();
      first.SetSeedToRandomValue(99);
      second.SetSeedToRandomValue(99);

      Take(first, 20).ShouldBe(Take(second, 20));
   }

   [Fact]
   public void RandomSeedLeavesTheDefaultSequence() {
      var reference = Take(new RandomSource(), 20);
      var reseeded = new RandomSource();
      reseeded.SetSeedToRandomValue();

      Take(reseeded, 20).ShouldNotBe(reference);
   }

   [Fact]
   public void SessionSeedChangesTheSequenceAndIsAppliedOnce() {
      var reference = new RandomSource();
      reference.SetSeedToRandomValue(42);
      var source = new RandomSource();

      ((ISessionSeedable) source).ApplySessionSeed(42);
      var first = Take(source, 5);
      ((ISessionSeedable) source).ApplySessionSeed(42);
      var second = Take(source, 5);

      first.ShouldBe(Take(reference, 5));
      second.ShouldBe(Take(reference, 5), "the second application must not restart the stream");
   }

   [Fact]
   public void SessionSeedIsForwardedToTheNullEvaluator() {
      var source = new NullableCountingSource();
      var evaluator = new AlwaysNullEvaluator();
      source.SetRandomNullEvaluator(evaluator);

      ((ISessionSeedable) source).ApplySessionSeed(4711);

      evaluator.SeedSeen.ShouldBe(4711);
   }

   [Fact]
   public void AnExplicitSeedWinsOverTheSessionSeed() {
      var reference = new RandomSource();
      reference.SetSeedToRandomValue(7);
      var source = new RandomSource();
      source.SetSeedToRandomValue(7);

      ((ISessionSeedable) source).ApplySessionSeed(42);

      Take(source, 5).ShouldBe(Take(reference, 5));
   }

   [Fact]
   public void AnExplicitSeedAfterTheSessionSeedStillApplies() {
      var reference = new RandomSource();
      reference.SetSeedToRandomValue(7);
      var source = new RandomSource();
      ((ISessionSeedable) source).ApplySessionSeed(42);

      source.SetSeedToRandomValue(7);

      Take(source, 5).ShouldBe(Take(reference, 5));
   }

   [Fact]
   public void InternalNextReturnsTheSameValuesAsNext() {
      IDataSource asInterface = new ConstantSource();
      asInterface.InternalNext(null).ShouldBe(7);
   }
}
