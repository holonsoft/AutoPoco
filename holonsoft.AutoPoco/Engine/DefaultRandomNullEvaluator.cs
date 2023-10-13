using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class DefaultRandomNullEvaluator : IRandomNullEvaluator {
   private Random _random = new(AutoPocoGlobalSettings.StandardSeed);

   public int ThresholdPercentage { get; set; } = AutoPocoGlobalSettings.NullCreationThreshold;

   public DefaultRandomNullEvaluator() { }

   public DefaultRandomNullEvaluator(int thresholdPercentage)
      => ThresholdPercentage = thresholdPercentage;

   public void SetSeedToRandomValue(int seed)
      => _random = new(seed);

   public bool ShouldNextValueReturnNull()
      => _random.Next(1, 100) <= ThresholdPercentage;
}
