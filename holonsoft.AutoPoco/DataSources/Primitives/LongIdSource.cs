// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerIdSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public class LongIdSource(long startValue = 0) : DataSourceBase<long> {
   /// <summary>
   ///   The current id.
   /// </summary>
   private long _currentId = startValue;

   protected override long GetNextValue(IGenerationContext? context) => _currentId++;
}