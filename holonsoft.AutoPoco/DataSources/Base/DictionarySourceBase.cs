using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class DictionarySourceBase : DataSourceBase<string> {
   private readonly bool _returnValueInsteadOfKey;

   public abstract Dictionary<string, string> Dictionary { get; }

   public DictionarySourceBase(bool returnValueInsteadOfKey, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      _returnValueInsteadOfKey = returnValueInsteadOfKey;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var num = Random.Next(0, Dictionary.Count);

      return _returnValueInsteadOfKey
         ? Dictionary.Values.ToList()[num]
         : Dictionary.Keys.ToList()[num];
   }
}
