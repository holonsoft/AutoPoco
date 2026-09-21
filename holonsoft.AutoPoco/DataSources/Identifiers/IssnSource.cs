using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an ISSN, the eight character number of a periodical, without the hyphen.
/// </summary>
/// <remarks>
///   Seven digits and a mod 11 check character that is an X when it stands for ten, the same arithmetic an
///   ISBN-10 uses with two digits more.
///   No hyphen, for the same reason an ISBN carries none here: an ISSN is written as four digits, a hyphen
///   and four characters, and while that split is fixed, leaving both numbers unhyphenated keeps the two
///   sources consistent. Insert the hyphen after position four when you need the printed form.
///   The numbers are syntactically valid, they are not registered and do not identify a real periodical.
/// </remarks>
public abstract class IssnSourceBase : DataSourceBase<string> {
   private const int _length = 8;

   protected IssnSourceBase(int? nullCreationThreshold = null)
      : base(nullCreationThreshold) { }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var characters = new char[_length];

      for (var i = 0; i < characters.Length - 1; i++)
         characters[i] = (char) ('0' + Random.Next(0, 10));

      var check = CheckDigits.Mod11Descending(characters, characters.Length - 1);
      characters[^1] = check == 10
         ? 'X'
         : (char) ('0' + check);

      return new string(characters);
   }
}

/// <summary>
///   An ISSN with a valid check character.
/// </summary>
public class IssnSource : IssnSourceBase {
   public IssnSource() : base(null) { }
}

/// <summary>
///   An ISSN that returns null every now and then.
/// </summary>
public class NullableIssnSource(int nullCreationThreshold) : IssnSourceBase(nullCreationThreshold) {
   public NullableIssnSource() : this(AutoPocoDefaults.NullCreationThreshold) { }
}
