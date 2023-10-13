using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Business;
public abstract class UrlSourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {

   private readonly RandomStringSource _randomStringSource = new(3, 10, 'a', 'z');

   private static readonly string[] _tlds = {
      "com", "org", "net", "gov", "edu", "mil", "int", "eu", "biz", "co.uk",
      "uk", "de", "jp","fr", "au", "ca", "us", "ru", "cn", "br", "in", "it", "nl", "pl", "se", "es", "ch", "at", "be", "dk",
      "no", "fi", "gr", "cz", "hu", "ro", "pt", "sk",  "bg", "hr","si", "rs", "ua", "tr", "me", "rs", "ba", "mk",  "al", "nz",
      "th", "sg", "my", "id", "ph", "vn", "hk", "tw", "kr", "ae", "sa", "il", "ma", "za", "eg", "ng", "ke", "pe", "ar", "cl",
      "mx","co","ve","ec","bo","cr","do","gt","sv","ni","pa","uy","py","by","kz","uz","tm","kg","tj","az", "ge","am","ua","md",
      "lv","lt","ee","su","vn","la","mm","kh","mv","np","af","pk","bd","lk","in","tr","ir","iq","qa","kw","sa","om","ye",
   };

   protected override string[] Data => _tlds;

   protected override string GetNextValue(IGenerationContext? context) {
      if (_nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return $"http://www.{_randomStringSource.Next(null)}.{_tlds[Random.Next(0, _tlds.Length - 1)]}";
   }
}

public class UrlSource : UrlSourceBase { }

public class NullableUrlSource(int nullCreationThreshold) : UrlSourceBase(nullCreationThreshold) {
   public NullableUrlSource() : this(AutoPocoGlobalSettings.NullCreationThreshold) { }
}