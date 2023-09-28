using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultDatetimeMemberConvention : ITypeFieldConvention, ITypePropertyConvention {
   public void Apply(ITypeFieldConventionContext context) 
      => context.SetValue(DateTime.MinValue);

   public void SpecifyRequirements(ITypeMemberConventionRequirements requirements) 
      => requirements.Type(x => x == typeof(DateTime));

   public void Apply(ITypePropertyConventionContext context) 
      => context.SetValue(DateTime.MinValue);
}