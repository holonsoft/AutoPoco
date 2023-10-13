// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultStringSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public class DefaultNullStringSource : DataSourceBase<string?> {
   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The <see cref="string" />.
   /// </returns>
   protected override string? GetNextValue(IGenerationContext? context)
      => null;
}
