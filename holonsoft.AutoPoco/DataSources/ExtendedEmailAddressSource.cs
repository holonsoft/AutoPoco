using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class ExtendedEmailAddressSource : DataSourceBase<string> {
   private readonly FirstNameSource _firstNameSource = new();
   private readonly LastNameSource _lastNameSource = new();

   public override void SetSeedToRandomValue() {
      base.SetSeedToRandomValue();
      _firstNameSource.SetSeedToRandomValue();
      _lastNameSource.SetSeedToRandomValue();
   }

   public override void SetSeedToRandomValue(int seed) {
      base.SetSeedToRandomValue(seed);
      _firstNameSource.SetSeedToRandomValue(seed);
      _lastNameSource.SetSeedToRandomValue(seed);
   }

   private static readonly string[] _domains = {
      "google.com",
      "msn.nl",
      "paspar.nl",
      "hotmail.com",
      "aol.com",
      "yahoo.com",
      "microsoft.com",
      "heise.de",
      "golem.de",
   };

   private readonly string _domain = "";

   public ExtendedEmailAddressSource() { }

   public ExtendedEmailAddressSource(string domain)
      => _domain = domain ?? "";

   public override string Next(IGenerationContext? context)
      => $"{_firstNameSource.Next(context)}.{_lastNameSource.Next(context)}@{(string.IsNullOrEmpty(_domain.Trim()) ? _domains[Random.Next(0, _domains.Length)] : _domain.Trim())}";
}
