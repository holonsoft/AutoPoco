using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class TypePropertyConventionContext(IEngineConfiguration config, IEngineConfigurationTypeMember member) 
   : TypeMemberConventionContext(config, member), ITypePropertyConventionContext {
   public new EngineTypePropertyMember Member => base.Member as EngineTypePropertyMember 
                                                   ?? throw new InvalidOperationException();
}