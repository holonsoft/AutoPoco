// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsStatesSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class StatesSourceBase(bool useAbbreviations) : DataSourceBase<string> {
   public abstract Dictionary<string, string> States { get; }

   public override string Next(IGenerationContext? context) {
      var num = Random.Next(0, States.Count);

      return useAbbreviations
         ? States.Values.ToList()[num]
         : States.Keys.ToList()[num];
   }
}
