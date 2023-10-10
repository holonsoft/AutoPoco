// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoremIpsumSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Properties;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   The lorem ipsum source.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="LoremIpsumSource" /> class.
/// </remarks>
/// <param name="count">
///   The count.
/// </param>
public class LoremIpsumSource(int count) : DataSourceBase<string>
{
    /// <summary>
    ///   The times.
    /// </summary>
    private readonly int _times = count;

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
        var builder = new StringBuilder(Resources.LoremIpsum);

        for (var i = 1; i < _times; i++)
            builder.AppendFormat("{0}\r\n\r\n", Resources.LoremIpsum);

        return builder.ToString().Trim();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LoremIpsumSource" /> class.
    /// </summary>
    public LoremIpsumSource()
      : this(1) { }
}