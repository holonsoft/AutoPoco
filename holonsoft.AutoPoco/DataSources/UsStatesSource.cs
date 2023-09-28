// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsStatesSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The us states source.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="UsStatesSource" /> class.
/// </remarks>
/// <param name="useAbbreviations">
///   The use abbreviations.
/// </param>
public class UsStatesSource(bool useAbbreviations) : StatesSourceBase(useAbbreviations) {
   private static readonly Dictionary<string, string> _states =
     new()
     {
         {"Alabama", "AL"},
         {"Alaska", "AK"},
         {"Arizona", "AZ"},
         {"Arkansas", "AR"},
         {"California", "CA"},
         {"Colorado", "CO"},
         {"Connecticut", "CT"},
         {"Delaware", "DE"},
         {"Florida", "FL"},
         {"Georgia", "GA"},
         {"Hawaii", "HI"},
         {"Idaho", "ID"},
         {"Illinois", "IL"},
         {"Indiana", "IN"},
         {"Iowa", "IA"},
         {"Kansas", "KS"},
         {"Kentucky", "KY"},
         {"Louisiana", "LA"},
         {"Maine", "ME"},
         {"Maryland", "MD"},
         {"Massachusetts", "MA"},
         {"Michigan", "MI"},
         {"Minnesota", "MN"},
         {"Mississippi", "MS"},
         {"Missouri", "MO"},
         {"Montana", "MT"},
         {"Nebraska", "NE"},
         {"Nevada", "NV"},
         {"New Hampshire", "NH"},
         {"New Jersey", "NJ"},
         {"New Mexico", "NM"},
         {"New York", "NY"},
         {"North Carolina", "NC"},
         {"North Dakota", "ND"},
         {"Ohio", "OH"},
         {"Oklahoma", "OK"},
         {"Oregon", "OR"},
         {"Pennsylvania", "PA"},
         {"Rhode Island", "RI"},
         {"South Carolina", "SC"},
         {"South Dakota", "SD"},
         {"Tennessee", "TN"},
         {"Texas", "TX"},
         {"Utah", "UT"},
         {"Vermont", "VT"},
         {"Virginia", "VA"},
         {"Washington", "WA"},
         {"West Virginia", "WV"},
         {"Wisconsin", "WI"},
         {"Wyoming", "WY"},
         {"District of Columbia", "DC"},
         {"Puerto Rico", "PR"},
         {"Guam", "GU"},
         {"U.S. Virgin Islands", "VI"},
         {"American Samoa", "AS"},
         {"Northern Mariana Islands", "MP"}
     };

   public override Dictionary<string, string> States => _states;

   public UsStatesSource()
      : this(false) { }
}

