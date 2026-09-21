using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an ISIN, the twelve character number of a security: a two letter country prefix, nine
///   characters of national number and a Luhn check digit.
/// </summary>
/// <remarks>
///   The check digit is not calculated over the characters directly. Every letter first becomes its position
///   in the alphabet plus nine, so A becomes 10 and Z becomes 35, the digits of those numbers are written out
///   one after another, and the Luhn check digit of that digit string closes the ISIN.
///   The numbers are syntactically valid, they are not registered with a numbering agency and do not identify
///   a real security.
/// </remarks>
public abstract class IsinSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   The prefixes a generated ISIN can start with: ISO 3166-1 alpha-2 codes of countries that have a
   ///   numbering agency, plus XS, the code the international agencies use for a security that belongs to no
   ///   single country. Real codes on purpose, an invented one would make the number impossible.
   /// </summary>
   private static readonly string[] _prefixes = [
      "AT", "AU", "BE", "CA", "CH", "CZ", "DE", "DK", "ES", "FI", "FR", "GB", "IE", "IT",
      "JP", "LU", "NL", "NO", "PL", "PT", "SE", "US", "XS"
   ];

   private const int _length = 12;
   private const string _nationalNumberAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

   private readonly string? _fixedPrefix;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="prefix">
   ///   Optional two letter country prefix, e.g. <c>DE</c>. Drawn from the known prefixes when null.
   /// </param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentException"><paramref name="prefix" /> is not two capital letters.</exception>
   protected IsinSourceBase(string? prefix = null, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (prefix is not null) {
         if (prefix.Length != 2 || prefix[0] is < 'A' or > 'Z' || prefix[1] is < 'A' or > 'Z')
            throw new ArgumentException(
               $"An ISIN prefix is two capital letters, e.g. 'DE', '{prefix}' is not.", nameof(prefix));

         _fixedPrefix = prefix;
      }
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var characters = new char[_length];
      var prefix = _fixedPrefix ?? _prefixes[Random.Next(0, _prefixes.Length)];
      prefix.AsSpan().CopyTo(characters);

      for (var i = prefix.Length; i < characters.Length - 1; i++)
         characters[i] = _nationalNumberAlphabet[Random.Next(0, _nationalNumberAlphabet.Length)];

      characters[^1] = (char) ('0' + LuhnOverExpandedCharacters(characters, characters.Length - 1));

      return new string(characters);
   }

   /// <summary>
   ///   Writes every letter out as its two digit value and takes the Luhn check digit of the result.
   /// </summary>
   private static int LuhnOverExpandedCharacters(ReadOnlySpan<char> characters, int dataLength) {
      // eleven characters, a letter becomes two digits, so twenty two digits are the most that can come out
      Span<char> digits = stackalloc char[dataLength * 2];
      var written = 0;

      for (var i = 0; i < dataLength; i++) {
         var c = characters[i];

         if (c is >= '0' and <= '9') {
            digits[written++] = c;
            continue;
         }

         var value = c - 'A' + 10;
         digits[written++] = (char) ('0' + (value / 10));
         digits[written++] = (char) ('0' + (value % 10));
      }

      return CheckDigits.Luhn(digits[..written], written);
   }
}

/// <summary>
///   An ISIN with a valid check digit.
/// </summary>
public class IsinSource(string? prefix) : IsinSourceBase(prefix) {
   public IsinSource() : this(null) { }
}

/// <summary>
///   An ISIN that returns null every now and then.
/// </summary>
public class NullableIsinSource(string? prefix, int nullCreationThreshold) : IsinSourceBase(prefix, nullCreationThreshold) {
   public NullableIsinSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIsinSource(string? prefix) : this(prefix, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIsinSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
