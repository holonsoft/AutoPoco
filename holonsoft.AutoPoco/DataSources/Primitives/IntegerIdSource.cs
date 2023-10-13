// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerIdSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   The integer id source.
/// </summary>
public class IntegerIdSource(int startValue = 0) : DataSourceBase<int> {
   /// <summary>
   ///   The current id.
   /// </summary>
   private int _currentId = startValue;

   protected override int GetNextValue(IGenerationContext? context) => _currentId++;
}