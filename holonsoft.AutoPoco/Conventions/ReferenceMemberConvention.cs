using System.Collections;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Conventions;

public class ReferenceMemberConvention : ITypeFieldConvention, ITypePropertyConvention {
   public void Apply(ITypeFieldConventionContext context) => context.SetSource(typeof(AutoSource<>).MakeGenericType(context.Member.FieldInfo.FieldType));

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements)
      => requirements.Type(x => x.IsClass
                                 && x.GetInterface(typeof(IEnumerable).FullName!) == null);

   public void Apply(ITypePropertyConventionContext context) 
      => context.SetSource(typeof(AutoSource<>).MakeGenericType(context.Member.PropertyInfo.PropertyType));
}