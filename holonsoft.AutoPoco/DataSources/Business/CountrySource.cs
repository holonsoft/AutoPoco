// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountrySource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Linq;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   The country source.
/// </summary>
public class CountrySource : FixedStringArraySourceBase
{
    private static readonly string[] _englishCountryNamesFromCultureList;
    protected override string[] Data => _englishCountryNamesFromCultureList;

    static CountrySource()
       => _englishCountryNamesFromCultureList = CultureInfo
          .GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
          .Where(x => !x.EnglishName.Contains(','))
          .Select(x => x.EnglishName)
          .Select(FormatEnglishName)
          .Distinct()
          .OrderBy(FormatEnglishName)
          .ToArray();

    private static string FormatEnglishName(string englishName)
    {
        var startIndex = englishName.IndexOf("(", StringComparison.Ordinal) + 1;
        var endIndex = englishName.IndexOf(")", StringComparison.Ordinal);

        if (startIndex > 0 && endIndex > startIndex)
            return englishName[startIndex..endIndex];

        return englishName;
    }
}