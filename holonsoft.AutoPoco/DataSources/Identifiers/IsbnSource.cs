using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   The two ISBN forms. The value of a member is the length of the number.
/// </summary>
public enum IsbnFormat {
   /// <summary>
   ///   The ten character form used until 2006. Its check digit can be an X, which stands for the value ten.
   /// </summary>
   Isbn10 = 10,

   /// <summary>
   ///   The thirteen digit form in use since 2007. Technically a GTIN-13 with the prefix 978 or 979.
   /// </summary>
   Isbn13 = 13
}

/// <summary>
///   Generates an ISBN with a valid check digit, without hyphens.
/// </summary>
/// <remarks>
///   No hyphens on purpose. An ISBN is grouped into prefix, registration group, registrant, publication and
///   check digit, and where those groups start depends on the registrant ranges that ISBN International
///   publishes. Without that data any hyphen we insert would sit in the wrong place, and a wrongly grouped
///   ISBN is worse test data than an ungrouped one.
///   The numbers are syntactically valid, they are not registered and do not identify a real book.
/// </remarks>
public abstract class IsbnSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   Prefix and registration group of an ISBN-13, only the combinations that are actually handed out:
   ///   978 with a single digit group (0 and 1 English, 2 French, 3 German, 4 Japan, 5 Russian, 7 China;
   ///   6 is not a single digit group), plus 979-8 for the United States and 979-10, 979-11 and 979-12 for
   ///   France, Korea and Italy.
   ///   979-0 is left out on purpose: that range belongs to the ISMN of sheet music and never carries an
   ///   ISBN, so a validator that knows the registration groups would throw such a number out.
   ///   The list is deliberately conservative, it holds no multi digit group (978-80 to 978-94, 978-600 and
   ///   so on). Leaving a group out only narrows the output, adding one that does not exist would break it.
   ///   Groups are allocated over time, the authoritative list is the range file of ISBN International at
   ///   https://www.isbn-international.org/range_file_generation. Last checked 2026-09-21.
   /// </summary>
   private static readonly string[] _isbn13GroupPrefixes =
      ["9780", "9781", "9782", "9783", "9784", "9785", "9787", "9798", "97910", "97911", "97912"];

   private readonly IsbnFormat _format;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="format">The ISBN form to generate.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="format" /> is not a known ISBN form.</exception>
   protected IsbnSourceBase(IsbnFormat format, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (!Enum.IsDefined(format))
         throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown ISBN format.");

      _format = format;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return _format == IsbnFormat.Isbn13
         ? NextIsbn13()
         : NextIsbn10();
   }

   /// <summary>
   ///   An allocated prefix and registration group, random digits for registrant and publication, and the
   ///   GS1 mod 10 check digit of a GTIN-13.
   /// </summary>
   private string NextIsbn13() {
      var digits = new char[(int) IsbnFormat.Isbn13];
      var groupPrefix = _isbn13GroupPrefixes[Random.Next(0, _isbn13GroupPrefixes.Length)];
      groupPrefix.AsSpan().CopyTo(digits);

      for (var i = groupPrefix.Length; i < digits.Length - 1; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      digits[^1] = (char) ('0' + CheckDigits.Gs1Mod10(digits, digits.Length - 1));

      return new string(digits);
   }

   /// <summary>
   ///   Nine random digits and the mod 11 check digit, which is an X when it stands for ten.
   /// </summary>
   private string NextIsbn10() {
      var digits = new char[(int) IsbnFormat.Isbn10];

      for (var i = 0; i < digits.Length - 1; i++)
         digits[i] = (char) ('0' + Random.Next(0, 10));

      var check = CheckDigits.Mod11Descending(digits, digits.Length - 1);
      digits[^1] = check == 10
         ? 'X'
         : (char) ('0' + check);

      return new string(digits);
   }
}

/// <summary>
///   An ISBN with a valid check digit, an ISBN-13 unless another form is given.
/// </summary>
public class IsbnSource(IsbnFormat format) : IsbnSourceBase(format) {
   public IsbnSource() : this(IsbnFormat.Isbn13) { }
}

/// <summary>
///   An ISBN with a valid check digit that returns null every now and then.
/// </summary>
public class NullableIsbnSource(IsbnFormat format, int nullCreationThreshold) : IsbnSourceBase(format, nullCreationThreshold) {
   public NullableIsbnSource() : this(IsbnFormat.Isbn13, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIsbnSource(IsbnFormat format) : this(format, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableIsbnSource(int nullCreationThreshold) : this(IsbnFormat.Isbn13, nullCreationThreshold) { }
}
