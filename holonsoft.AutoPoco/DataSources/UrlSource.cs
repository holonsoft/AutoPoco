using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class UrlSource : DataSourceBase<string> {

   private readonly RandomStringSource _randomStringSource = new(3, 10, 'a', 'z');

   private static readonly string[] _tlds = {
      ".com",
      ".de",
      ".nl",
      ".eu",
      ".net",
      "co.uk",
      ".org",
   };

   public override void SetSeedToRandomValue() {
      base.SetSeedToRandomValue();
      _randomStringSource.SetSeedToRandomValue();
   }

   public override void SetSeedToRandomValue(int seed) {
      base.SetSeedToRandomValue(seed);
      _randomStringSource.SetSeedToRandomValue(seed);
   }

   public override string Next(IGenerationContext? context)
      => $"http://www.{_randomStringSource.Next(null)}{_tlds[Random.Next(0, _tlds.Length)]}";
}
