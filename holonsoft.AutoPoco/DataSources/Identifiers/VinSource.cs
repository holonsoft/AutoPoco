using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates a VIN, the seventeen character vehicle identification number, with a valid North American
///   check digit at position nine.
/// </summary>
/// <remarks>
///   The alphabet leaves out I, O and Q, which a VIN never contains, and position ten leaves out U, Z and
///   the zero on top, the model year convention of 49 CFR 565. The check digit transliterates every letter
///   to a value, weights the seventeen positions 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 and
///   takes the sum mod 11, a ten written as an X. Strictly this check digit is the North American scheme;
///   European VINs are not obliged to carry one, but a number that satisfies it is valid everywhere.
///   A prefix pins the leading characters, e.g. a real world manufacturer identifier such as WVW, so a fleet
///   looks like one manufacturer. The numbers are syntactically valid, they are not registered and do not
///   identify a real vehicle.
/// </remarks>
public abstract class VinSourceBase : DataSourceBase<string> {
   private const int _length = 17;
   private const int _checkDigitPosition = 8;

   /// <summary>
   ///   Every character a VIN may contain: digits and capital letters without I, O and Q.
   /// </summary>
   private const string _alphabet = "0123456789ABCDEFGHJKLMNPRSTUVWXYZ";

   /// <summary>
   ///   The characters allowed at position ten, the model year: the VIN alphabet without U, Z and zero.
   /// </summary>
   private const string _modelYearAlphabet = "123456789ABCDEFGHJKLMNPRSTVWXY";

   private static readonly int[] _weights = [8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2];

   private readonly string _prefix;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="prefix">
   ///   Optional leading characters, normally the three character world manufacturer identifier. At most
   ///   eight characters, everything up to the check digit; the check digit is always calculated.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException">
   ///   <paramref name="prefix" /> contains a character a VIN never carries, or is longer than eight characters.
   /// </exception>
   protected VinSourceBase(string? prefix = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      _prefix = prefix ?? "";

      foreach (var c in _prefix)
         if (!_alphabet.Contains(c))
            throw new ArgumentException(
               $"A VIN consists of digits and capital letters without I, O and Q, '{_prefix}' does not.", nameof(prefix));

      if (_prefix.Length > _checkDigitPosition)
         throw new ArgumentException(
            $"A VIN prefix is at most {_checkDigitPosition} characters long, everything up to the check digit; '{_prefix}' has {_prefix.Length}.",
            nameof(prefix));
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var characters = new char[_length];
      _prefix.AsSpan().CopyTo(characters);

      for (var i = _prefix.Length; i < _length; i++)
         characters[i] = i switch {
            _checkDigitPosition => '0',      // placeholder, calculated below
            9 => _modelYearAlphabet[Random.Next(0, _modelYearAlphabet.Length)],
            _ => _alphabet[Random.Next(0, _alphabet.Length)]
         };

      var sum = 0;
      for (var i = 0; i < _length; i++)
         sum += TransliterationValue(characters[i]) * _weights[i];

      var check = sum % 11;
      characters[_checkDigitPosition] = check == 10
         ? 'X'
         : (char) ('0' + check);

      return new string(characters);
   }

   /// <summary>
   ///   The value a character counts in the check digit sum: a digit counts as itself, a letter by the VIN
   ///   transliteration table, where A to H are one to eight, J to N one to five, P is seven, R is nine and
   ///   S to Z are two to nine.
   /// </summary>
   private static int TransliterationValue(char c)
      => c switch {
         >= '0' and <= '9' => c - '0',
         >= 'A' and <= 'H' => c - 'A' + 1,
         >= 'J' and <= 'N' => c - 'J' + 1,
         'P' => 7,
         'R' => 9,
         _ => c - 'S' + 2
      };
}

/// <summary>
///   A VIN with a valid check digit.
/// </summary>
public class VinSource(string? prefix) : VinSourceBase(prefix) {
   public VinSource() : this(null) { }
}

/// <summary>
///   A VIN that returns null every now and then.
/// </summary>
public class NullableVinSource(string? prefix, int nullCreationThreshold) : VinSourceBase(prefix, nullCreationThreshold) {
   public NullableVinSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableVinSource(string? prefix) : this(prefix, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableVinSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
