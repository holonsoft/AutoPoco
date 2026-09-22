using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   Generates an SKU, the article code a merchant assigns, from a pattern. The default
///   <c>@@@-#####</c> gives something like <c>KDX-83741</c>.
/// </summary>
/// <remarks>
///   An SKU follows no standard, every merchant invents their own scheme, so this source makes no claim of
///   validity beyond the pattern; give your own pattern when your system expects a different shape, see
///   <see cref="PatternSourceBase" /> for the placeholders. For the article numbers that do follow a
///   standard, GTIN, EAN or ASIN, use those sources instead.
/// </remarks>
public abstract class SkuSourceBase(string? pattern, int? nullCreationThreshold = null)
   : PatternSourceBase(pattern ?? DefaultPattern, nullCreationThreshold) {
   /// <summary>
   ///   The pattern used when none is given: three letters, a hyphen and five digits.
   /// </summary>
   public const string DefaultPattern = "@@@-#####";
}

/// <summary>
///   An SKU from a pattern, three letters, a hyphen and five digits unless another pattern is given.
/// </summary>
public class SkuSource(string? pattern) : SkuSourceBase(pattern) {
   public SkuSource() : this(null) { }
}

/// <summary>
///   An SKU that returns null every now and then.
/// </summary>
public class NullableSkuSource(string? pattern, int nullCreationThreshold) : SkuSourceBase(pattern, nullCreationThreshold) {
   public NullableSkuSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSkuSource(string? pattern) : this(pattern, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSkuSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
