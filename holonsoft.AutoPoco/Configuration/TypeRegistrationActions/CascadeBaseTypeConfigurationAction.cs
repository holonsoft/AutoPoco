using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class CascadeBaseTypeConfigurationAction(IEngineConfiguration configuration) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type) => ApplyToType(type);

   private void ApplyToType(IEngineConfigurationType type) {
      // Create the dependency stack
      var membersToApply = GetAllTypeHierarchyMembers(configuration, type);

      foreach (var existingMemberConfig in membersToApply) {
         var currentMemberConfig = type.GetRegisteredMember(existingMemberConfig.Member);
         if (currentMemberConfig != null) continue;

         type.RegisterMember(existingMemberConfig.Member);
         currentMemberConfig = type.GetRegisteredMember(existingMemberConfig.Member);
         currentMemberConfig.SetDataSources(existingMemberConfig.GetDataSources());
      }
   }

   protected virtual IEnumerable<IEngineConfigurationTypeMember> GetAllTypeHierarchyMembers(
     IEngineConfiguration baseConfiguration, IEngineConfigurationType sourceType) {
      var configurationStack = new Stack<IEngineConfigurationType>();
      var currentType = sourceType.RegisteredType;
      IEngineConfigurationType? currentTypeConfiguration = null;

      // Get all the base types into a stack, where the base-most type is at the top
      while (currentType != null) {
         currentTypeConfiguration = baseConfiguration.GetRegisteredType(currentType);
         if (currentTypeConfiguration != null)
            configurationStack.Push(currentTypeConfiguration);
         currentType = currentType.BaseType;
      }

      // Put all the implemented interfaces on top of that
      foreach (var interfaceType in sourceType.RegisteredType.GetInterfaces()) {
         currentTypeConfiguration = baseConfiguration.GetRegisteredType(interfaceType);
         if (currentTypeConfiguration != null)
            configurationStack.Push(currentTypeConfiguration);
      }

      var membersToApply = (from typeConfig in configurationStack
                            from memberConfig in typeConfig.GetRegisteredMembers()
                            select memberConfig).ToArray();

      return membersToApply;
   }
}