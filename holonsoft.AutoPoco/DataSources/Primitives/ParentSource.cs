// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParentSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   The parent source.
/// </summary>
/// <typeparam name="T">The Type of parent.</typeparam>
public class ParentSource<T> : DataSourceBase<T>
{

    /// <summary>
    ///   The next.
    /// </summary>
    /// <param name="context">The context.</param>
    public override T Next(IGenerationContext? context)
       => FindParent(context?.Node, false)!; // Search upwards for parent

    /// <summary>
    /// Find parent node if any
    /// </summary>
    /// <param name="current">Current node</param>
    /// <param name="foundOne">The found one</param>
    /// <returns>T depending on generic type</returns>
    private static T? FindParent(IGenerationContextNode? current, bool foundOne)
    {
        if (current == null)
            return default;

        if (current.ContextType != GenerationTargetTypes.Object)
            return FindParent(current.Parent, foundOne);

        var type = (TypeGenerationContextNode)current;

        if (type.Target is not T t)
            return FindParent(current.Parent, true);

        return foundOne
           ? t
           : FindParent(current.Parent, true);
    }
}