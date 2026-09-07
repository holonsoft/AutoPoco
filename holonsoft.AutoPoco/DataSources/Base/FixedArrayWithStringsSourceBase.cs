using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class FixedArrayWithStringsSourceBase : DataSourceBase<string> {
   protected abstract string[] Data { get; }

   public FixedArrayWithStringsSourceBase(int? nullCreationThreshold = null)
      : base(nullCreationThreshold) { }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return Data[Random.Next(0, Data.Length)];
   }
}

