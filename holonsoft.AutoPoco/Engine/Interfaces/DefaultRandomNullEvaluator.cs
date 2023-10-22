using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Engine.Interfaces;

public class DefaultRandomNullEvaluator : IRandomNullEvaluator
{
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
