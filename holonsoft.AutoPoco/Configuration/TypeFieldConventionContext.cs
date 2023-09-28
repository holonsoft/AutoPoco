using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class TypeFieldConventionContext(IEngineConfiguration config, IEngineConfigurationTypeMember member) 
   : TypeMemberConventionContext(config, member), ITypeFieldConventionContext {
   public new EngineTypeFieldMember Member => base.Member as EngineTypeFieldMember 
                                                ?? throw new InvalidOperationException();
}