using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Business;
public abstract class ExtendedEmailAddressSourceBase(int? nullCreationThreshold = null, params string[] domains) : DataSourceBase<string>(nullCreationThreshold) {
   private const string _firstNamePurpose = "ExtendedEmailAddressSource#firstName";
   private const string _lastNamePurpose = "ExtendedEmailAddressSource#lastName";

   // own streams from the start, otherwise both names follow the same sequence of indices into their catalog
   private readonly FirstNameSource _firstNameSource = NestedSource.Seeded(new FirstNameSource(), _firstNamePurpose);
   private readonly LastNameSource _lastNameSource = NestedSource.Seeded(new LastNameSource(), _lastNamePurpose);

   private readonly string[] _domainsOfCaller = domains;

   public override void SetSeedToRandomValue() {
      base.SetSeedToRandomValue();
      _firstNameSource.SetSeedToRandomValue();
      _lastNameSource.SetSeedToRandomValue();
   }

   public override void SetSeedToRandomValue(int seed) {
      base.SetSeedToRandomValue(seed);
      _firstNameSource.SetSeedToRandomValue(NestedSource.Seed(seed, _firstNamePurpose));
      _lastNameSource.SetSeedToRandomValue(NestedSource.Seed(seed, _lastNamePurpose));
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
      if (NullCreationThreshold.HasValue) {
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
   public NullableExtendedEmailAddressSource() : this(AutoPocoDefaults.NullCreationThreshold) { }

   public NullableExtendedEmailAddressSource(params string[] domains) : this(AutoPocoDefaults.NullCreationThreshold, domains) { }
}
