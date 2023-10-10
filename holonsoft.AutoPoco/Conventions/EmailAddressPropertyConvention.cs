using System.Globalization;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Conventions;

public class EmailAddressPropertyConvention : ITypePropertyConvention {
   public void Apply(ITypePropertyConventionContext context) => context.SetSource<EmailAddressSource>();

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) {
      requirements.Name(x => string.Compare(x, "EmailAddress", true, CultureInfo.InvariantCulture) == 0);
      requirements.Type(x => x == typeof(string));
   }
}