// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class ColorSourceBase<T>(byte rangeStart, byte rangeEnd) : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object)
         Color.FromArgb(
            Random.Next(rangeStart, rangeEnd),
            Random.Next(rangeStart, rangeEnd),
            Random.Next(rangeStart, rangeEnd));
}

public class ColorSource(byte rangeStart, byte rangeEnd) : ColorSourceBase<Color>(rangeStart, rangeEnd) {
   public ColorSource()
      : this(0, 255) { }
}

public class NullableColorSource(byte rangeStart, byte rangeEnd) : ColorSourceBase<Color?>(rangeStart, rangeEnd) {
   public NullableColorSource()
      : this(0, 255) { }
}
