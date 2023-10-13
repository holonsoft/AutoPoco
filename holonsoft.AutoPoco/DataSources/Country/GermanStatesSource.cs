using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Country;

public abstract class GermanStatesSourceBase(bool useAbbreviations, int? nullCreationThreshold = null) : DictionarySourceBase(useAbbreviations, nullCreationThreshold) {
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

   public override Dictionary<string, string> Dictionary => _states;
}

public class GermanStatesSource(bool useAbbreviations) : GermanStatesSourceBase(useAbbreviations, null) {
   public GermanStatesSource() : this(false) { }
}

public class NullableGermanStatesSource(bool useAbbreviations, int nullCreationThreshold) : GermanStatesSourceBase(useAbbreviations, nullCreationThreshold) {
   public NullableGermanStatesSource() : this(false, AutoPocoGlobalSettings.NullCreationThreshold) { }
}
