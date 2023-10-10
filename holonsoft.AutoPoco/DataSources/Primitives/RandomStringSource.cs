// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomStringSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   The random string source.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="RandomStringSource" /> class.
/// </remarks>
/// <param name="minLength">
///   The min.
/// </param>
/// <param name="maxLength">
///   The max.
/// </param>
/// <param name="minChar"></param>
/// <param name="maxChar"></param>
public class RandomStringSource(int minLength, int maxLength, char minChar, char maxChar) : DataSourceBase<string>
{

    public RandomStringSource(int minLength, int maxLength)
       : this(minLength, maxLength, (char)65, (char)123) { }

    public RandomStringSource()
       : this(5, 10, 'A', 'z') { }

    /// <summary>
    ///   The next.
    /// </summary>
    /// <param name="context">
    ///   The context.
    /// </param>
    /// <returns>
    ///   The <see cref="string" />.
    /// </returns>
    public override string Next(IGenerationContext? context)
    {
        var builder = new StringBuilder();
        var length = Random.Next(minLength, maxLength + 1);

        for (var x = 0; x < length; x++)
        {
            var value = Random.Next(minChar, maxChar);
            builder.Append((char)value);
        }

        return builder.ToString();
    }
}