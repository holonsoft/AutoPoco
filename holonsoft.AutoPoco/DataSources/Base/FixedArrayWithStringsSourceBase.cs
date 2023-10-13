using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class FixedArrayWithStringsSourceBase : DataSourceBase<string> {
   protected readonly int? _nullCreationThreshold;

   protected abstract string[] Data { get; }

   public FixedArrayWithStringsSourceBase(int? nullCreationThreshold = null) {
      RandomNullEvaluator.SetSeedToRandomValue(nullCreationThreshold ?? AutoPocoGlobalSettings.NullCreationThreshold);
      _nullCreationThreshold = nullCreationThreshold;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (_nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return Data[Random.Next(0, Data.Length)];
   }
}

