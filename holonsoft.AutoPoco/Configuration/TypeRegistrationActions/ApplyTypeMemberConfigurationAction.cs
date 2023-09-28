using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class ApplyTypeMemberConfigurationAction(IEngineConfigurationProvider configurationProvider) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type) => ApplyToType(type);

#pragma warning disable CA1822 // Mark members as static
   private void ApplyToType(IEngineConfigurationType type) {
      var typeProviders = configurationProvider.GetConfigurationTypes()
        .Where(x => x.GetConfigurationType() == type.RegisteredType);

      foreach (var typeProvider in typeProviders)
         foreach (var memberProvider in typeProvider.GetConfigurationMembers()) {
            var typeMember = memberProvider.GetConfigurationMember();

            // Get the member
            var configuredMember = type.GetRegisteredMember(typeMember ?? throw new InvalidOperationException());

            // Set the action on that member if a dataSource has been set explicitly for this member
            var dataSources = memberProvider.GetDataSources().ToArray();
            if (dataSources.Any())
               configuredMember!.SetDataSources(dataSources);
         }
   }
#pragma warning restore CA1822 // Mark members as static
}