using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultIntegerMemberConvention : ITypeFieldConvention, ITypePropertyConvention {
   public void Apply(ITypeFieldConventionContext context) {
      if (context.Member.FieldInfo.FieldType == typeof(int))
         context.SetValue(0);
   }

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Type(x => x == typeof(int));

   public void Apply(ITypePropertyConventionContext context) {
      if (context.Member.PropertyInfo.PropertyType == typeof(int))
         context.SetValue(0);
   }
}