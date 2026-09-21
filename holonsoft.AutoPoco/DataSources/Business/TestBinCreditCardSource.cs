using holonsoft.AutoPoco.Configuration;
using static holonsoft.AutoPoco.DataSources.Business.CreditCardSourceBase;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   A credit card number that is built on one of the card numbers the payment processors publish for their
///   test environments, so it is recognisable as test data instead of possibly being somebody's card.
/// </summary>
/// <remarks>
///   <see cref="CreditCardSourceBase" /> draws everything behind the single scheme digit, so a number it
///   produces can fall inside a range a real issuer actually uses. Nothing is broken about that, a valid
///   check digit is no secret and a number alone buys nobody anything without the security code, the name,
///   the expiry date and, these days, a strong customer authentication. Two things still speak for this
///   source. A generated number can coincide with a card that really exists, and a database full of numbers
///   that pass the Luhn check sets off every scanner that looks for card data.
///   This source therefore keeps the first six digits of a published test number and draws only the rest.
///   Be precise about what that buys: those numbers are a convention of the payment industry, published by
///   Stripe, Mastercard, Cybersource and others, and they are recognised as test data almost everywhere.
///   They are not a range that a standard reserves, ISO/IEC 7812 knows no such range for cards. So a number
///   from here is very unlikely to belong to anybody, it is not impossible by decree.
/// </remarks>
public abstract class TestBinCreditCardSourceBase(CreditCardType preferred, int? nullCreationThreshold = null)
   : CreditCardSourceBase(preferred, nullCreationThreshold) {
   /// <summary>
   ///   The first six digits of card numbers that the payment processors publish as test numbers, grouped by
   ///   the scheme they belong to. Every entry keeps the scheme digit of its type, so a Visa still starts
   ///   with a four.
   /// </summary>
   private static readonly Dictionary<CreditCardType, string[]> _publishedTestBins = new() {
      [CreditCardType.Visa] = ["411111", "424242", "401288", "400005"],
      [CreditCardType.MasterCard] = ["555555", "510510", "550000", "222300"],
      [CreditCardType.AmericanExpress] = ["378282", "371449", "378734"],
      [CreditCardType.Discover] = ["601111", "601100", "601198"]
   };

   /// <summary>
   ///   Every test BIN this source can draw, for a caller that wants to recognise the numbers again.
   /// </summary>
   public static IReadOnlyCollection<string> PublishedTestBins { get; } =
      [.. _publishedTestBins.Values.SelectMany(b => b).OrderBy(b => b, StringComparer.Ordinal)];

   protected override string GetIssuerIdentification(CreditCardType cardType) {
      if (!_publishedTestBins.TryGetValue(cardType, out var bins))
         throw new InvalidOperationException($"Credit card type '{cardType}' has no published test number.");

      return bins[Random.Next(0, bins.Length)];
   }
}

/// <summary>
///   A credit card number on a published test BIN.
/// </summary>
public class TestBinCreditCardSource(CreditCardType creditCardType) : TestBinCreditCardSourceBase(creditCardType) {
   public TestBinCreditCardSource() : this(CreditCardType.Random) { }
}

/// <summary>
///   A credit card number on a published test BIN that returns null every now and then.
/// </summary>
public class NullableTestBinCreditCardSource(CreditCardType creditCardType, int nullCreationThreshold)
   : TestBinCreditCardSourceBase(creditCardType, nullCreationThreshold) {
   public NullableTestBinCreditCardSource() : this(CreditCardType.Random, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableTestBinCreditCardSource(CreditCardType creditCardType)
      : this(creditCardType, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableTestBinCreditCardSource(int nullCreationThreshold)
      : this(CreditCardType.Random, nullCreationThreshold) { }
}
