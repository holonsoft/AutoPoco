using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class EmailAddressSourceBase(string namePartPrefix, string domain, int? nullCreationThreshold = null) : DataSourceBase<string> {
   private int _index;

   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      return $"{namePartPrefix}{_index++}@{domain}";
   }
}

/// <summary>
/// Generate pseudo email address, tld is according to RFC2606 for empty constructor
/// </summary>
/// <param name="namePartPrefix">namepart of email</param>
/// <param name="domain">domain part of email</param>
public class EmailAddressSource(string namePartPrefix, string domain) : EmailAddressSourceBase(namePartPrefix, domain) {
   public EmailAddressSource() : this("eg", "example.test") { }
}

public class NullableEmailAddressSource(string namePartPrefix, string domain, int nullCreationThreshold) : EmailAddressSourceBase(namePartPrefix, domain, nullCreationThreshold) {
   public NullableEmailAddressSource() : this("eg", "example.test", AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableEmailAddressSource(string namePartPrefix, string domain) : this(namePartPrefix, domain, AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableEmailAddressSource(int nullCreationThreshold) : this("eg", "example.test", nullCreationThreshold) { }
}
