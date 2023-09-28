using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class RegisterTypeMembersFromConfigurationAction(IEngineConfigurationProvider configurationProvider) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type) => ApplyToType(type);

   private void ApplyToType(IEngineConfigurationType type) {
      var typeProviders = configurationProvider.GetConfigurationTypes()
        .Where(x => x.GetConfigurationType() == type.RegisteredType);
      foreach (var typeProvider in typeProviders)
         foreach (var member in typeProvider.GetConfigurationMembers()) {
            var typeMember = member.GetConfigurationMember();

            if (type.GetRegisteredMember(typeMember ?? throw new InvalidOperationException()) == null)
               type.RegisterMember(typeMember);
         }
   }
}