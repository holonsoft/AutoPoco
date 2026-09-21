using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an IMEI, the fifteen digit serial number of a mobile device, with a valid Luhn check digit.
/// </summary>
/// <remarks>
///   The first eight digits are the Type Allocation Code, which says who built the device and which model it
///   is. A drawn TAC is not one the GSMA has handed out, so give the TAC as a prefix when the numbers have to
///   look like one model.
///   The numbers are syntactically valid, they are not registered and do not identify a real device.
/// </remarks>
public abstract class ImeiSourceBase : DataSourceBase<string> {
   private const int _length = 15;

   private readonly string _typeAllocationCode;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="typeAllocationCode">
   ///   Optional leading digits, normally the eight digit Type Allocation Code. At most fourteen digits, the
   ///   check digit is always generated.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException">
   ///   <paramref name="typeAllocationCode" /> contains something other than digits, or is too long.
   /// </exception>
   protected ImeiSourceBase(string? typeAllocationCode = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      _typeAllocationCode = typeAllocationCode ?? "";

      foreach (var c in _typeAllocationCode)
         if (c is < '0' or > '9')
            throw new ArgumentException(
               $"A type allocation code consists of digits only, '{_typeAllocationCode}' does not.", nameof(typeAllocationCode));

      if (_typeAllocationCode.Length > _length - 1)
         throw new ArgumentException(
            $"An IMEI has {_length} digits including the check digit, so the part given is at most {_length - 1} digits long, '{_typeAllocationCode}' has {_typeAllocationCode.Length}.",
            nameof(typeAllocationCode));
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var digits = new char[_length];
      _typeAllocationCode.AsSpan().CopyTo(digits);

      for (var i = _typeAllocationCode.Length; i < digits.Length - 1; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[^1] = (char) ('0' + CheckDigits.Luhn(digits, digits.Length - 1));

      return new string(digits);
   }
}

/// <summary>
///   An IMEI with a valid check digit.
/// </summary>
public class ImeiSource(string? typeAllocationCode) : ImeiSourceBase(typeAllocationCode) {
   public ImeiSource() : this(null) { }
}

/// <summary>
///   An IMEI that returns null every now and then.
/// </summary>
public class NullableImeiSource(string? typeAllocationCode, int nullCreationThreshold)
   : ImeiSourceBase(typeAllocationCode, nullCreationThreshold) {
   public NullableImeiSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableImeiSource(string? typeAllocationCode) : this(typeAllocationCode, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableImeiSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
