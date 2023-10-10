// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   The credit card source.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="CreditCardSource" /> class.
/// </remarks>
/// <param name="preferred">
///   The preferred.
/// </param>
public class CreditCardSource(CreditCardSource.CreditCardType preferred) : DataSourceBase<string>
{
    /// <summary>
    ///   The credit card type.
    /// </summary>
    public enum CreditCardType
    {
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

    /// <summary>
    ///   The m preferred.
    /// </summary>
    private readonly CreditCardType _preferred = preferred;

    /// <summary>
    ///   Initializes a new instance of the <see cref="CreditCardSource" /> class.
    /// </summary>
    public CreditCardSource()
       : this(CreditCardType.Random)
    {
    }

    /// <summary>
    ///   The next.
    /// </summary>
    /// <param name="context">
    ///   The context.
    /// </param>
    /// <returns>
    ///   The <see cref="string" />.
    /// </returns>
    public override string Next(IGenerationContext? context)
    {
        var cardType = _preferred;

        if (_preferred == CreditCardType.Random)
            cardType = (CreditCardType)RandomNumberGenerator.Current.Next(1, 4);

        return cardType switch
        {
            CreditCardType.AmericanExpress => FormatAmexCardNumber(GenerateCreditCardNumber(3, 15)),
            CreditCardType.Discover => FormatCreditCardNumber(GenerateCreditCardNumber(6, 16)),
            CreditCardType.MasterCard => FormatCreditCardNumber(GenerateCreditCardNumber(5, 16)),
            CreditCardType.Visa => FormatCreditCardNumber(GenerateCreditCardNumber(4, 16)),
            _ => null,
        } ?? throw new InvalidOperationException();
    }

    private static string GenerateCreditCardNumber(int prefix, int length)
    {
        var cardNumber = new StringBuilder(prefix.ToString());
        while (cardNumber.Length < length - 1)
            cardNumber.Append(RandomNumberGenerator.Current.Next(0, 10));

        cardNumber.Append(CalculateLuhnDigit(cardNumber.ToString()));
        return cardNumber.ToString();
    }

    private static int CalculateLuhnDigit(string number)
    {
        var sum = 0;
        var isEven = false;
        for (var i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';
            if (isEven)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            isEven = !isEven;
        }

        return (10 - sum % 10) % 10;
    }

    private static string FormatCreditCardNumber(string number)
    {
        StringBuilder formattedNumber = new();
        for (var i = 0; i < number.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                formattedNumber.Append(' ');
            }

            formattedNumber.Append(number[i]);
        }

        return formattedNumber.ToString();
    }

    private static string FormatAmexCardNumber(string number)
    {
        StringBuilder formattedNumber = new();
        for (var i = 0; i < number.Length; i++)
        {
            if (i is 4 or 10)
            {
                formattedNumber.Append(' ');
            }

            formattedNumber.Append(number[i]);
        }

        return formattedNumber.ToString();
    }
}