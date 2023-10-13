using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class DictionarySourceBase : DataSourceBase<string> {
   private readonly bool _returnValueInsteadOfKey;
   private readonly int? _nullCreationThreshold;

   public abstract Dictionary<string, string> Dictionary { get; }

   public DictionarySourceBase(bool returnValueInsteadOfKey, int? nullCreationThreshold = null) {
      RandomNullEvaluator.SetSeedToRandomValue(nullCreationThreshold ?? AutoPocoGlobalSettings.NullCreationThreshold);
      _returnValueInsteadOfKey = returnValueInsteadOfKey;
      _nullCreationThreshold = nullCreationThreshold;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (_nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var num = Random.Next(0, Dictionary.Count);

      return _returnValueInsteadOfKey
         ? Dictionary.Values.ToList()[num]
         : Dictionary.Keys.ToList()[num];
   }
}
