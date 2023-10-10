using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

public class GermanStatesSource(bool useAbbreviations) : StatesSourceBase(useAbbreviations)
{
    /// <summary>
    ///   The states.
    /// </summary>
    private static readonly Dictionary<string, string> _states = new() {
         {"Baden-Württemberg", "BW"},
         {"Bayern", "BY"},
         {"Berlin", "BE"},
         {"Brandenburg", "BB"},
         {"Bremen", "HB"},
         {"Hamburg", "HH"},
         {"Hessen", "HE"},
         {"Mecklenburg-Vorpommern", "MV"},
         {"Niedersachsen", "NI"},
         {"Nordrhein-Westfalen", "NW"},
         {"Rheinland-Pfalz", "RP"},
         {"Saarland", "SL"},
         {"Sachsen", "SN"},
         {"Sachsen-Anhalt", "ST"},
         {"Schleswig-Holstein", "SH"},
         {"Thüringen", "TH"}
     };

    public override Dictionary<string, string> States => _states;

    /// <summary>
    ///   Initializes a new instance of the <see cref="UsStatesSource" /> class.
    /// </summary>
    public GermanStatesSource()
      : this(false) { }
}