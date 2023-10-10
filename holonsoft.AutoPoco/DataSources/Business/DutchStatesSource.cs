using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

public class DutchStatesSource(bool useAbbreviations) : StatesSourceBase(useAbbreviations)
{
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