using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   The GTIN variants. The value of a member is the total length of the number, the check digit included.
/// </summary>
public enum GtinFormat {
   /// <summary>
   ///   GTIN-8, also known as EAN-8. Used where a GTIN-13 does not fit, e.g. on small packages.
   /// </summary>
   Gtin8 = 8,

   /// <summary>
   ///   GTIN-12, also known as UPC-A. The form used in North America.
   /// </summary>
   Gtin12 = 12,

   /// <summary>
   ///   GTIN-13, also known as EAN-13. The form printed on retail articles in Europe.
   /// </summary>
   Gtin13 = 13,

   /// <summary>
   ///   GTIN-14, also known as ITF-14 or SCC-14. Identifies a trade unit such as a carton of articles.
   /// </summary>
   Gtin14 = 14
}

/// <summary>
///   Generates a GTIN (Global Trade Item Number) with a valid GS1 mod 10 check digit, as digits without
///   separators. An optional prefix fixes the leading digits, so a whole catalogue can share one GS1
///   company prefix and still get a different article number per item.
/// </summary>
/// <remarks>
///   The numbers are syntactically valid, they are not registered with GS1 and do not identify a real
///   article. Do not use them outside of test data.
///   Without a prefix the leading digits are drawn freely, so a number can land in a range GS1 reserves for
///   a special purpose, e.g. 02 and 2 for goods weighed in the shop, 977 for periodicals, 978 and 979 for
///   books or 98 for coupons. The first digit of a GTIN-14 is the indicator digit, where 0 stands for a
///   GTIN-13 padded with a zero and 9 for a variable measure item. Give a prefix when that matters.
/// </remarks>
public abstract class GtinSourceBase : DataSourceBase<string> {
   private readonly GtinFormat _format;
   private readonly string _prefix;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="format">The GTIN variant to generate.</param>
   /// <param name="prefix">
   ///   Optional leading digits, e.g. a GS1 company prefix. At most one digit shorter than the format,
   ///   the check digit is always generated. A prefix that long leaves nothing to draw, so the source
   ///   returns the same number on every call.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="format" /> is not a known GTIN variant.</exception>
   /// <exception cref="ArgumentException"><paramref name="prefix" /> contains something other than digits, or is too long for the format.</exception>
   protected GtinSourceBase(GtinFormat format, string? prefix = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (!Enum.IsDefined(format))
         throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown GTIN format.");

      _prefix = prefix ?? "";

      foreach (var c in _prefix)
         if (c is < '0' or > '9')
            throw new ArgumentException($"A GTIN prefix consists of digits only, '{_prefix}' does not.", nameof(prefix));

      var length = (int) format;
      if (_prefix.Length > length - 1)
         throw new ArgumentException(
            $"A {format} has {length} digits including the check digit, so its prefix is at most {length - 1} digits long, '{_prefix}' has {_prefix.Length}.",
            nameof(prefix));

      _format = format;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var digits = new char[(int) _format];
      _prefix.AsSpan().CopyTo(digits);

      for (var i = _prefix.Length; i < digits.Length - 1; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[^1] = (char) ('0' + CheckDigits.Gs1Mod10(digits, digits.Length - 1));

      return new string(digits);
   }
}

/// <summary>
///   A GTIN with a valid check digit, a GTIN-13 unless another format is given.
/// </summary>
public class GtinSource(GtinFormat format, string? prefix) : GtinSourceBase(format, prefix) {
   public GtinSource() : this(GtinFormat.Gtin13, null) { }

   public GtinSource(GtinFormat format) : this(format, null) { }
}

/// <summary>
///   A GTIN with a valid check digit that returns null every now and then.
/// </summary>
public class NullableGtinSource(GtinFormat format, string? prefix, int nullCreationThreshold)
   : GtinSourceBase(format, prefix, nullCreationThreshold) {
   public NullableGtinSource() : this(GtinFormat.Gtin13, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableGtinSource(GtinFormat format) : this(format, null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableGtinSource(GtinFormat format, string? prefix) : this(format, prefix, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableGtinSource(int nullCreationThreshold) : this(GtinFormat.Gtin13, null, nullCreationThreshold) { }
}

/// <summary>
///   The EAN-13 of a retail article, the GTIN-13 under its everyday name.
/// </summary>
public class Ean13Source(string? prefix) : GtinSourceBase(GtinFormat.Gtin13, prefix) {
   public Ean13Source() : this(null) { }
}

/// <summary>
///   The EAN-13 of a retail article, returns null every now and then.
/// </summary>
public class NullableEan13Source(string? prefix, int nullCreationThreshold)
   : GtinSourceBase(GtinFormat.Gtin13, prefix, nullCreationThreshold) {
   public NullableEan13Source() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableEan13Source(string? prefix) : this(prefix, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableEan13Source(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
