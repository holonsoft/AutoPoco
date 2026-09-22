using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Geo;

/// <summary>
///   The shared shape of the latitude and longitude sources: a double drawn uniformly between two bounds
///   that have to stay inside the valid range of the coordinate.
/// </summary>
/// <remarks>
///   A latitude and a longitude on the same object are drawn independently, so together they name a point
///   inside the bounding box, but not a point with any further meaning; it can lie in the sea. Where the
///   two members have to describe one meaningful place, build them with Impose from prepared coordinates.
/// </remarks>
public abstract class GeoCoordinateSourceBase<T> : DataSourceBase<T> {
   private readonly double _min;
   private readonly double _max;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="min">Lower bound, inclusive.</param>
   /// <param name="max">Upper bound, inclusive.</param>
   /// <param name="limit">The magnitude the coordinate can never exceed, 90 for a latitude, 180 for a longitude.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   A bound is not finite, lies outside the valid range, or the maximum lies below the minimum.
   /// </exception>
   private protected GeoCoordinateSourceBase(double min, double max, double limit, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (!double.IsFinite(min) || min < -limit || min > limit)
         throw new ArgumentOutOfRangeException(nameof(min), min, $"The bound must lie between {-limit} and {limit}.");

      if (!double.IsFinite(max) || max < -limit || max > limit)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The bound must lie between {-limit} and {limit}.");

      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum must not be below the minimum ({min}).");

      _min = min;
      _max = max;
   }

   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public double Min => _min;

   /// <summary>
   ///   Upper bound, inclusive.
   /// </summary>
   public double Max => _max;

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextBetween(_min, _max);
}

/// <summary>
///   A latitude in decimal degrees, the whole world unless a bounding box is given.
/// </summary>
public class LatitudeSource(double min, double max) : GeoCoordinateSourceBase<double>(min, max, 90) {
   public LatitudeSource() : this(-90, 90) { }
}

/// <summary>
///   A latitude that returns null every now and then.
/// </summary>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults" />
public class NullableLatitudeSource(double min, double max) : GeoCoordinateSourceBase<double?>(min, max, 90) {
   public NullableLatitudeSource() : this(-90, 90) { }
}

/// <summary>
///   A longitude in decimal degrees, the whole world unless a bounding box is given.
/// </summary>
public class LongitudeSource(double min, double max) : GeoCoordinateSourceBase<double>(min, max, 180) {
   public LongitudeSource() : this(-180, 180) { }
}

/// <summary>
///   A longitude that returns null every now and then.
/// </summary>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults" />
public class NullableLongitudeSource(double min, double max) : GeoCoordinateSourceBase<double?>(min, max, 180) {
   public NullableLongitudeSource() : this(-180, 180) { }
}
