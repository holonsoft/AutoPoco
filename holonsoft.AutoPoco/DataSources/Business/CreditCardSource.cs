using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;
using static holonsoft.AutoPoco.DataSources.Business.CreditCardSourceBase;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class CreditCardSourceBase(CreditCardSourceBase.CreditCardType preferred, int? nullCreationThreshold = null) : DataSourceBase<string>(nullCreationThreshold) {
   /// <summary>
   ///   The credit card type.
   /// </summary>
   public enum CreditCardType {
      /// <summary>
      ///   The random.
      /// </summary>
      Random = 0,

      /// <summary>
      ///   The master card.
      /// </summary>
      MasterCard = 1,

      /// <summary>
      ///   The visa.
      /// </summary>
      Visa = 2,

      /// <summary>
      ///   The american express.
      /// </summary>
      AmericanExpress = 3,

      /// <summary>
      ///   The discover.
      /// </summary>
      Discover = 4
   }

   private readonly CreditCardType _preferred = preferred;

   /// <summary>
   ///   Every card type that <see cref="CreditCardType.Random" /> can pick, taken from the enum itself
   ///   so that a type added later is drawn as well.
   /// </summary>
   private static readonly CreditCardType[] _selectableTypes =
      [.. Enum.GetValues<CreditCardType>().Where(x => x != CreditCardType.Random)];

   public CreditCardSourceBase()
      : this(CreditCardType.Random) {
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var cardType = _preferred;

      if (_preferred == CreditCardType.Random)
         cardType = _selectableTypes[Random.Next(_selectableTypes.Length)];

      var number = GenerateCreditCardNumber(GetIssuerIdentification(cardType), LengthOf(cardType));

      return cardType == CreditCardType.AmericanExpress
         ? FormatAmexCardNumber(number)
         : FormatCreditCardNumber(number);
   }

   /// <summary>
   ///   The digits a number of this card type starts with. This class draws a prefix inside a range the
   ///   scheme really issues in, so a validator that detects the scheme from the number agrees with the
   ///   requested card type. Everything behind the prefix is drawn freely, so the number can still land on a
   ///   range a real issuer uses. A derived source returns a longer prefix, see <c>TestBinCreditCardSource</c>.
   /// </summary>
   /// <exception cref="InvalidOperationException"><paramref name="cardType" /> is not a card type with a known format.</exception>
   protected virtual string GetIssuerIdentification(CreditCardType cardType)
      => cardType switch {
         // 34 and 37 belong to American Express; 35 is JCB and 36 is Diners Club, so the second digit is not free
         CreditCardType.AmericanExpress => Random.Next(0, 2) == 0 ? "34" : "37",
         // Discover issues in 6011, 644 to 649 and 65; the rest of the 6 range belongs to other schemes
         CreditCardType.Discover => Random.Next(0, 3) switch {
            0 => "6011",
            1 => $"64{Random.Next(4, 10)}",
            _ => "65"
         },
         // 51 to 55, plus the 2 series 2221 to 2720 that Mastercard hands out since 2017
         CreditCardType.MasterCard => Random.Next(0, 2) == 0
            ? $"5{Random.Next(1, 6)}"
            : (2221 + Random.Next(0, 500)).ToString(),
         // every number that starts with a 4 is a Visa
         CreditCardType.Visa => "4",
         _ => throw new InvalidOperationException($"Credit card type '{cardType}' has no number format.")
      };

   /// <summary>
   ///   The total length of a number of this card type, the check digit included.
   /// </summary>
   protected static int LengthOf(CreditCardType cardType)
      => cardType == CreditCardType.AmericanExpress
         ? 15
         : 16;

   private string GenerateCreditCardNumber(string prefix, int length) {
      var cardNumber = new StringBuilder(prefix);
      while (cardNumber.Length < length - 1)
         cardNumber.Append(Random.Next(0, 10));

      var partial = cardNumber.ToString();
      cardNumber.Append(CheckDigits.Luhn(partial, partial.Length));
      return cardNumber.ToString();
   }

   private static string FormatCreditCardNumber(string number) {
      StringBuilder formattedNumber = new();
      for (var i = 0; i < number.Length; i++) {
         if (i > 0 && i % 4 == 0)
            formattedNumber.Append(' ');

         formattedNumber.Append(number[i]);
      }

      return formattedNumber.ToString();
   }

   private static string FormatAmexCardNumber(string number) {
      StringBuilder formattedNumber = new();
      for (var i = 0; i < number.Length; i++) {
         if (i is 4 or 10)
            formattedNumber.Append(' ');

         formattedNumber.Append(number[i]);
      }

      return formattedNumber.ToString();
   }
}

public class CreditCardSource(CreditCardType creditCardType) : CreditCardSourceBase(creditCardType) {
   public CreditCardSource() : this(CreditCardType.Random) { }
}

public class NullableCreditCardSource(CreditCardType creditCardType, int nullCreationThreshold) : CreditCardSourceBase(creditCardType, nullCreationThreshold) {
   public NullableCreditCardSource() : this(CreditCardType.Random, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableCreditCardSource(CreditCardType creditCardType) : this(creditCardType, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableCreditCardSource(int nullCreationThreshold) : this(CreditCardType.Random, nullCreationThreshold) { }
}
