using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultStringMemberConvention : ITypeFieldConvention, ITypePropertyConvention {
   public void Apply(ITypeFieldConventionContext context) {
      if (context.Member.FieldInfo.FieldType == typeof(string))
         context.SetValue("");
   }

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) => requirements.Type(x => x == typeof(string));

   public void Apply(ITypePropertyConventionContext context) {
      if (context.Member.PropertyInfo.PropertyType == typeof(string))
         context.SetValue("");
   }
}