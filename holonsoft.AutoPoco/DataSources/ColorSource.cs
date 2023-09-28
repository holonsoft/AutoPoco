// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The color source.
/// </summary>
public class ColorSource(byte rangeStart, byte rangeEnd) : DataSourceBase<Color> {
   /// <summary>
   ///   Initializes a new instance of the <see cref="ColorSource" /> class.
   /// </summary>
   public ColorSource()
      : this(0, 255) { }

   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The <see cref="Color" />.
   /// </returns>
   public override Color Next(IGenerationContext? context)
      => Color.FromArgb(
        RandomNumberGenerator.Current.Next(rangeStart, rangeEnd),
        RandomNumberGenerator.Current.Next(rangeStart, rangeEnd),
        RandomNumberGenerator.Current.Next(rangeStart, rangeEnd));

}