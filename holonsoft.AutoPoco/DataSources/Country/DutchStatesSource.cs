using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Country;

public abstract class DutchStatesSourceBase(bool useAbbreviations, int? nullCreationThreshold = null) : DictionarySourceBase(useAbbreviations, nullCreationThreshold) {
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

   public override Dictionary<string, string> Dictionary => _states;
}

public class DutchStatesSource(bool useAbbreviations) : DutchStatesSourceBase(useAbbreviations, null) {
   public DutchStatesSource() : this(false) { }
}

public class NullableDutchStatesSource(bool useAbbreviations, int nullCreationThreshold) : DutchStatesSourceBase(useAbbreviations, nullCreationThreshold) {
   public NullableDutchStatesSource() : this(false, AutoPocoGlobalSettings.NullCreationThreshold) { }
}