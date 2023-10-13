﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountrySource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Country;

public abstract class CountrySourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {
   private static readonly string[] _englishCountryNamesFromCultureList;
   protected override string[] Data => _englishCountryNamesFromCultureList;

   static CountrySourceBase()
      => _englishCountryNamesFromCultureList = CultureInfo
         .GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
         .Where(x => !x.EnglishName.Contains(','))
         .Select(x => x.EnglishName)
         .Select(FormatEnglishName)
         .Distinct()
         .OrderBy(FormatEnglishName)
         .ToArray();

   private static string FormatEnglishName(string englishName) {
      var startIndex = englishName.IndexOf("(", StringComparison.Ordinal) + 1;
      var endIndex = englishName.IndexOf(")", StringComparison.Ordinal);

      if (startIndex > 0 && endIndex > startIndex)
         return englishName[startIndex..endIndex];

      return englishName;
   }
}

/// <summary>
///   The country source. Generated by reading all cultures 
/// </summary>
public class CountrySource : CountrySourceBase {
   public CountrySource() { }
}

/// <summary>
///   The country source. Generated by reading all cultures 
///   Result can be NULL, too 
/// </summary>
public class NullableCountrySource : CountrySourceBase {
   public NullableCountrySource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableCountrySource(int nullCreationThreshold) : base(nullCreationThreshold) { }
}
