using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   Generates a string from a pattern: a <c>#</c> becomes a digit, a <c>@</c> a capital letter, a <c>*</c>
///   one of both, a backslash keeps the next character literal, and every other character stays as it is.
///   <c>"@@@-#####"</c> gives something like <c>KDX-83741</c>.
/// </summary>
/// <remarks>
///   Meant for the identifiers that follow no public standard: an SKU, a serial number, an order code.
///   Because there is no standard, nothing here claims to be validatable; the pattern is the contract.
///   For numbers that do carry a published check digit, use the dedicated sources, they exist for exactly
///   that reason.
/// </remarks>
public abstract class PatternSourceBase : DataSourceBase<string> {
   private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
   private const string _lettersAndDigits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

   private readonly string _pattern;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="pattern">
   ///   The pattern: <c>#</c> digit, <c>@</c> capital letter, <c>*</c> digit or capital letter, backslash
   ///   escapes, everything else literal.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="pattern" /> is null, empty or ends in a lone backslash.</exception>
   protected PatternSourceBase(string pattern, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (string.IsNullOrEmpty(pattern))
         throw new ArgumentException("A pattern needs at least one character.", nameof(pattern));

      var escaped = false;
      foreach (var c in pattern)
         escaped = !escaped && c == '\\';

      if (escaped)
         throw new ArgumentException($"The pattern '{pattern}' ends in a lone backslash, which escapes nothing.", nameof(pattern));

      _pattern = pattern;
   }

   /// <summary>
   ///   The pattern this source was created with.
   /// </summary>
   public string Pattern => _pattern;

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return FillPattern(_pattern);
   }

   /// <summary>
   ///   Replaces every placeholder of the pattern with a drawn character.
   /// </summary>
   protected string FillPattern(string pattern) {
      var value = new StringBuilder(pattern.Length);
      var escaped = false;

      foreach (var c in pattern) {
         if (escaped) {
            value.Append(c);
            escaped = false;
            continue;
         }

         switch (c) {
            case '\\':
               escaped = true;
               break;
            case '#':
               value.Append((char) ('0' + Random.Next(0, 10)));
               break;
            case '@':
               value.Append(_letters[Random.Next(0, _letters.Length)]);
               break;
            case '*':
               value.Append(_lettersAndDigits[Random.Next(0, _lettersAndDigits.Length)]);
               break;
            default:
               value.Append(c);
               break;
         }
      }

      return value.ToString();
   }
}

/// <summary>
///   A string built from a pattern: <c>#</c> digit, <c>@</c> capital letter, <c>*</c> both, backslash escapes.
/// </summary>
public class PatternSource(string pattern) : PatternSourceBase(pattern);

/// <summary>
///   A string built from a pattern that returns null every now and then.
/// </summary>
public class NullablePatternSource(string pattern, int nullCreationThreshold) : PatternSourceBase(pattern, nullCreationThreshold) {
   public NullablePatternSource(string pattern) : this(pattern, AutoPocoDefaults.NullCreationThreshold) { }
}
