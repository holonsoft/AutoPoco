using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine.Interfaces;

public class DefaultRandomNullEvaluator : IRandomNullEvaluator
{
    private Random _random = new StableRandom(AutoPocoDefaults.Seed);

    public int ThresholdPercentage { get; set; } = AutoPocoDefaults.NullCreationThreshold;

    public DefaultRandomNullEvaluator() { }

    public DefaultRandomNullEvaluator(int thresholdPercentage)
       => ThresholdPercentage = thresholdPercentage;

    public void SetSeedToRandomValue(int seed)
       => _random = new StableRandom(seed);

    public bool ShouldNextValueReturnNull()
       => _random.Next(1, 100) <= ThresholdPercentage;
}
