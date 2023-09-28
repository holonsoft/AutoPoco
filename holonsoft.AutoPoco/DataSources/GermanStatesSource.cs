using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources;

public class GermanStatesSource(bool useAbbreviations) : StatesSourceBase(useAbbreviations) {
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

public class DutchStatesSource(bool useAbbreviations) : StatesSourceBase(useAbbreviations) {
   /// <summary>
   ///   The states.
   /// </summary>
   private static readonly Dictionary<string, string> _states = new() {
         {"Drenthe", "DR"},
         {"Flevoland", "FL"},
         {"Friesland", "FR"},
         {"Gelderland", "GE"},
         {"Groningen", "GR"},
         {"Limburg", "LI"},
         {"Noord-Brabant", "NB"},
         {"Noord-Holland", "NH"},
         {"Overijssel", "OV"},
         {"Utrecht", "UT"},
         {"Zeeland", "ZE"},
         {"Zuid-Holland", "ZH"}
     };

   public override Dictionary<string, string> States => _states;

   /// <summary>
   ///   Initializes a new instance of the <see cref="UsStatesSource" /> class.
   /// </summary>
   public DutchStatesSource()
     : this(false) { }
}