namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IRandomNullEvaluator {
   bool ShouldNextValueReturnNull();
   void SetSeedToRandomValue(int seed);
}
