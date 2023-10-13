// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   The default source.
/// </summary>
/// <typeparam name="T">The type to generate the default value of</typeparam>
public class DefaultSource<T> : DataSourceBase<T> {
#pragma warning disable CS1723
   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The <see cref="T" />.
   /// </returns>
   protected override T GetNextValue(IGenerationContext? context)
      => (typeof(T).IsValueType
         ? default
         : Activator.CreateInstance<T>())
            ?? throw new InvalidOperationException();
}