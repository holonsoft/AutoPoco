using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   Generates an ASIN, the ten character article number Amazon assigns: a leading B followed by nine
///   digits or capital letters.
/// </summary>
/// <remarks>
///   An ASIN carries no check digit, so unlike a GTIN or an ISBN it cannot be validated on its own.
///   Books are the exception, Amazon uses their ISBN-10 as the ASIN, see <see cref="IsbnSource" />.
///   Most ASINs in circulation today happen to start with B0, that is how Amazon allocates them at the
///   moment and not part of the format, so this source does not narrow the second character.
///   The numbers are syntactically valid, they do not identify a real article.
/// </remarks>
public abstract class AsinSourceBase : DataSourceBase<string> {
   /// <summary>
   ///   The characters an ASIN is built from, digits and capital letters without any exclusion.
   /// </summary>
   private const string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

   private const int _length = 10;

   protected AsinSourceBase(int? nullCreationThreshold = null)
      : base(nullCreationThreshold) { }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var characters = new char[_length];
      characters[0] = 'B';

      for (var i = 1; i < _length; i++)
         characters[i] = _alphabet[Random.Next(0, _alphabet.Length)];

      return new string(characters);
   }
}

/// <summary>
///   An ASIN, the article number Amazon assigns.
/// </summary>
public class AsinSource() : AsinSourceBase(null);

/// <summary>
///   An ASIN that returns null every now and then.
/// </summary>
public class NullableAsinSource(int nullCreationThreshold) : AsinSourceBase(nullCreationThreshold) {
   public NullableAsinSource() : this(AutoPocoDefaults.NullCreationThreshold) { }
}
