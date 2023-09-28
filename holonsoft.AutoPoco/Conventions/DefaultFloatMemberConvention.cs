using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultFloatMemberConvention : ITypeFieldConvention, ITypePropertyConvention {
   public void Apply(ITypeFieldConventionContext context) {
      if (context.Member.FieldInfo.FieldType == typeof(float))
         context.SetValue(0);
   }

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Type(x => x == typeof(float));

   public void Apply(ITypePropertyConventionContext context) {
      if (context.Member.PropertyInfo.PropertyType == typeof(float))
         context.SetValue(0);
   }
}