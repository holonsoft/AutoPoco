using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using static holonsoft.AutoPoco.DataSources.Business.CreditCardSourceBase;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class CreditCardSourceBase(CreditCardSourceBase.CreditCardType preferred, int? nullCreationThreshold = null) : DataSourceBase<string>() {
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

   public CreditCardSourceBase()
      : this(CreditCardType.Random) {
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var cardType = _preferred;

      if (_preferred == CreditCardType.Random)
         cardType = (CreditCardType) Random.Next(1, 4);

      return cardType switch {
         CreditCardType.AmericanExpress => FormatAmexCardNumber(GenerateCreditCardNumber(3, 15)),
         CreditCardType.Discover => FormatCreditCardNumber(GenerateCreditCardNumber(6, 16)),
         CreditCardType.MasterCard => FormatCreditCardNumber(GenerateCreditCardNumber(5, 16)),
         CreditCardType.Visa => FormatCreditCardNumber(GenerateCreditCardNumber(4, 16)),
         _ => null,
      } ?? throw new InvalidOperationException();
   }

   private string GenerateCreditCardNumber(int prefix, int length) {
      var cardNumber = new StringBuilder(prefix.ToString());
      while (cardNumber.Length < length - 1)
         cardNumber.Append(Random.Next(0, 10));

      cardNumber.Append(CalculateLuhnDigit(cardNumber.ToString()));
      return cardNumber.ToString();
   }

   private static int CalculateLuhnDigit(string number) {
      var sum = 0;
      var isEven = false;
      for (var i = number.Length - 1; i >= 0; i--) {
         var digit = number[i] - '0';
         if (isEven) {
            digit *= 2;
            if (digit > 9)
               digit -= 9;
         }

         sum += digit;
         isEven = !isEven;
      }

      return (10 - (sum % 10)) % 10;
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
   public NullableCreditCardSource() : this(CreditCardType.Random, AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableCreditCardSource(CreditCardType creditCardType) : this(creditCardType, AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableCreditCardSource(int nullCreationThreshold) : this(CreditCardType.Random, nullCreationThreshold) { }
}
