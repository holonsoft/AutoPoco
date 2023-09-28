// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerIdSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The integer id source.
/// </summary>
public class IntegerIdSource : DataSourceBase<int> {
   /// <summary>
   ///   The current id.
   /// </summary>
   private int _currentId;

   public override int Next(IGenerationContext? context) => _currentId++;
}