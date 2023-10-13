// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtorSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   The ctor source.
/// </summary>
/// <typeparam name="T">The type to create</typeparam>
/// <remarks>
///   Initializes a new instance of the <see cref="CtorSource{T}" /> class.
/// </remarks>
/// <param name="constructor">The constructor.</param>
public class CtorSource<T>(ConstructorInfo constructor) : DataSourceBase<T> {

   /// <summary>
   ///   The m constructor info.
   /// </summary>
   private readonly ConstructorInfo _constructorInfo = constructor;

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
   protected override T GetNextValue(IGenerationContext? context) {
      // TODO: May actually create a parallel set of interfaces for doing non-generic requests to session
      // This would negate the need for that awful reflection
      var args = _constructorInfo.GetParameters().Select(
        x => {
           var method = context!.GetType().GetMethod("Next", Type.EmptyTypes);
           var target = method?.MakeGenericMethod(x.ParameterType);
           return target?.Invoke(context, null);
        }).ToArray();

      return (T) _constructorInfo.Invoke(args);
   }
#pragma warning restore CS1723
}