using System.Drawing;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class ColorSourceBase<T>(byte alpha, byte rangeStartRed, byte rangeEndRed, byte rangeStartGreen, byte rangeEndGreen, byte rangeStartBlue, byte rangeEndBlue) : DataSourceBase<T> {
   public byte Alpha { get; private set; } = alpha;
   public byte RangeStartRed { get; private set; } = rangeStartRed;
   public byte RangeEndRed { get; private set; } = rangeEndRed;
   public byte RangeStartGreen { get; private set; } = rangeStartGreen;
   public byte RangeEndGreen { get; private set; } = rangeEndGreen;
   public byte RangeStartBlue { get; private set; } = rangeStartBlue;
   public byte RangeEndBlue { get; private set; } = rangeEndBlue;

   public ColorSourceBase<T> SetColorRange(byte alpha, byte rangeStartRed, byte rangeEndRed, byte rangeStartGreen, byte rangeEndGreen, byte rangeStartBlue, byte rangeEndBlue) {
      Alpha = alpha;
      RangeStartRed = rangeStartRed;
      RangeEndRed = rangeEndRed;
      RangeStartGreen = rangeStartGreen;
      RangeEndGreen = rangeEndGreen;
      RangeStartBlue = rangeStartBlue;
      RangeEndBlue = rangeEndBlue;
      return this;
   }

   public ColorSourceBase<T> SetColorRange(byte rangeStart, byte rangeEnd)
      => SetColorRange(255, rangeStart, rangeEnd, rangeStart, rangeEnd, rangeStart, rangeEnd);

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object)
         Color.FromArgb(
            Random.Next(RangeStartRed, RangeEndRed),
            Random.Next(RangeStartGreen, RangeEndGreen),
            Random.Next(RangeStartBlue, RangeEndBlue));
}

/// <summary>
/// Creates a simple color source. For more complex colors use SetColorRange() 
/// </summary>
/// <param name="rangeStart"></param>
/// <param name="rangeEnd"></param>
public class ColorSource(byte rangeStart, byte rangeEnd) : ColorSourceBase<Color>(255, rangeStart, rangeEnd, rangeStart, rangeEnd, rangeStart, rangeEnd) {
   public ColorSource()
      : this(0, 255) { }
}

/// <summary>
/// Creates a simple nullable color source. For more complex colors use SetColorRange() 
/// </summary>
/// <param name="rangeStart"></param>
/// <param name="rangeEnd"></param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableColorSource(byte rangeStart, byte rangeEnd) : ColorSourceBase<Color?>(255, rangeStart, rangeEnd, rangeStart, rangeEnd, rangeStart, rangeEnd) {
   public NullableColorSource()
      : this(0, 255) { }
}
