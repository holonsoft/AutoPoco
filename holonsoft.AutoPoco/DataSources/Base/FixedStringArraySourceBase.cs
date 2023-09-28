// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsStatesSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public abstract class FixedStringArraySourceBase : DataSourceBase<string> {
   protected abstract string[] Data { get; }

   public override string Next(IGenerationContext? context)
      => Data[Random.Next(0, Data.Length)];
}

