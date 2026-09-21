using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an SSCC (Serial Shipping Container Code), the eighteen digit number that identifies a single
///   logistic unit such as a pallet or a parcel. Carries the same GS1 mod 10 check digit as a GTIN.
/// </summary>
/// <remarks>
///   An SSCC starts with an extension digit, then the GS1 company prefix, then the serial reference of the
///   unit, then the check digit. Only the check digit is calculated here, so give a prefix when the numbers
///   have to look like they come from one company.
///   The numbers are syntactically valid, they are not registered with GS1 and do not identify a real unit.
/// </remarks>
public abstract class SsccSourceBase : DataSourceBase<string> {
   private const int _length = 18;

   private readonly string _prefix;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="prefix">
   ///   Optional leading digits, e.g. an extension digit together with a GS1 company prefix. At most
   ///   seventeen digits, the check digit is always generated.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="prefix" /> contains something other than digits, or is too long.</exception>
   protected SsccSourceBase(string? prefix = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      _prefix = prefix ?? "";

      foreach (var c in _prefix)
         if (c is < '0' or > '9')
            throw new ArgumentException($"An SSCC prefix consists of digits only, '{_prefix}' does not.", nameof(prefix));

      if (_prefix.Length > _length - 1)
         throw new ArgumentException(
            $"An SSCC has {_length} digits including the check digit, so its prefix is at most {_length - 1} digits long, '{_prefix}' has {_prefix.Length}.",
            nameof(prefix));
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var digits = new char[_length];
      _prefix.AsSpan().CopyTo(digits);

      for (var i = _prefix.Length; i < digits.Length - 1; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[^1] = (char) ('0' + CheckDigits.Gs1Mod10(digits, digits.Length - 1));

      return new string(digits);
   }
}

/// <summary>
///   An SSCC with a valid check digit.
/// </summary>
public class SsccSource(string? prefix) : SsccSourceBase(prefix) {
   public SsccSource() : this(null) { }
}

/// <summary>
///   An SSCC that returns null every now and then.
/// </summary>
public class NullableSsccSource(string? prefix, int nullCreationThreshold) : SsccSourceBase(prefix, nullCreationThreshold) {
   public NullableSsccSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSsccSource(string? prefix) : this(prefix, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSsccSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
