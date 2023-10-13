using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Business;
public abstract class ExtendedEmailAddressSourceBase(int? nullCreationThreshold = null, params string[] domains) : DataSourceBase<string> {
   private readonly FirstNameSource _firstNameSource = new();
   private readonly LastNameSource _lastNameSource = new();

   private readonly string[] _domainsOfCaller = domains;

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

   /// <summary>
   /// According to RFC2606
   /// </summary>
   private static readonly string[] _someProvidedDomains = {
      "google.invalid",
      "msn.invalid",
      "paspar.invalid",
      "hotmail.invalid",
      "aol.invalid",
      "yahoo.invalid",
      "holonsoft.invalid",
      "microsoft.invalid",
      "heise.invalid",
      "golem.invalid",
      "google.test",
      "msn.test",
      "paspar.test",
      "hotmail.test",
      "aol.test",
      "yahoo.test",
      "holonsoft.test",
      "microsoft.test",
      "heise.test",
      "golem.test",
      "google.example",
      "msn.example",
      "paspar.example",
      "hotmail.example",
      "aol.example",
      "yahoo.example",
      "holonsoft.example",
      "microsoft.example",
      "heise.example",
      "golem.example",
   };

   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return $"{_firstNameSource.Next(context)}.{_lastNameSource.Next(context)}@{GetDomain()}";

      string GetDomain() {
         var domainSource = (_domainsOfCaller == null || _domainsOfCaller.Length == 0)
                              ? _someProvidedDomains
                              : _domainsOfCaller;

         return domainSource[Random.Next(0, domainSource.Length)];
      }
   }
}

public class ExtendedEmailAddressSource(params string[] domains) : ExtendedEmailAddressSourceBase(null, domains) { }

public class NullableExtendedEmailAddressSource(int nullCreationThreshold, params string[] domains) : ExtendedEmailAddressSourceBase(nullCreationThreshold, domains) {
   public NullableExtendedEmailAddressSource() : this(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableExtendedEmailAddressSource(params string[] domains) : this(AutoPocoGlobalSettings.NullCreationThreshold, domains) { }
}
