using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Engine;

/// <summary>
///   Controls whether the engine turns nullable member annotations into random null values.
///   When enabled, every property or field whose declared type allows null (<c>string?</c>, <c>int?</c>, ...)
///   gets null with the given probability instead of a value from its data source.
///   Members without a nullable annotation and members set by <c>Impose</c> are never touched.
/// </summary>
public sealed class NullableAnnotationSettings {
   /// <summary>
   ///   The engine ignores nullable annotations. This is the default.
   /// </summary>
   public static NullableAnnotationSettings Disabled { get; } = new(false, 0);

   public NullableAnnotationSettings(bool respectNullableAnnotations, int nullCreationThreshold) {
      ArgumentOutOfRangeException.ThrowIfNegative(nullCreationThreshold);
      ArgumentOutOfRangeException.ThrowIfGreaterThan(nullCreationThreshold, 100);

      RespectNullableAnnotations = respectNullableAnnotations;
      NullCreationThreshold = nullCreationThreshold;
   }

   /// <summary>
   ///   Creates enabled settings. A null threshold uses <see cref="AutoPocoDefaults.NullCreationThreshold" />.
   /// </summary>
   /// <param name="nullCreationThreshold">probability in percent (0 to 100) that a nullable member becomes null</param>
   public static NullableAnnotationSettings Enabled(int? nullCreationThreshold = null)
      => new(true, nullCreationThreshold ?? AutoPocoDefaults.NullCreationThreshold);

   /// <summary>
   ///   True when nullable annotations drive null generation.
   /// </summary>
   public bool RespectNullableAnnotations { get; }

   /// <summary>
   ///   Probability in percent that a nullable member becomes null.
   /// </summary>
   public int NullCreationThreshold { get; }
}
