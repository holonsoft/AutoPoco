using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

namespace holonsoft.AutoPoco.Conventions;

// Still a klooge, but leaves me in a better place for later
public class DefaultEngineConfigurationProviderLoadingConvention : IEngineConfigurationProviderLoader {
   public void Apply(IEngineConfigurationProviderLoaderContext context) {
      OnPreTypeLoad(context);

      var typeAction = CreateTypeRegistrationActions(context);
      foreach (var type in context.Configuration.GetRegisteredTypes())
         typeAction.Apply(type);

      OnPostTypeLoad(context);
   }

   protected virtual void OnPreTypeLoad(IEngineConfigurationProviderLoaderContext context) => FindAndRegisterAllBaseTypes(context);

   protected virtual void OnPostTypeLoad(IEngineConfigurationProviderLoaderContext context) {
      foreach (var type in context.Configuration.GetRegisteredTypes())
         ApplyBaseRulesToType(context, type);
   }

   protected virtual void FindAndRegisterAllBaseTypes(IEngineConfigurationProviderLoaderContext context) {
      foreach (var type in context.ConfigurationProvider.GetConfigurationTypes())
         TryRegisterType(context.Configuration, type.GetConfigurationType());
   }

   protected virtual void TryRegisterType(IEngineConfiguration configuration, Type configType) {
      var configuredType = configuration.GetRegisteredType(configType);
      if (configuredType == null) {
         configuration.RegisterType(configType);
         configuredType = configuration.GetRegisteredType(configType);
      }

      foreach (var interfaceType in configType.GetInterfaces())
         TryRegisterType(configuration, interfaceType);

      var baseType = configType.BaseType;
      if (baseType != null)
         TryRegisterType(configuration, baseType);
   }

   protected virtual void ApplyBaseRulesToType(IEngineConfigurationProviderLoaderContext context,
     IEngineConfigurationType type) {
      var membersToApply = GetAllTypeHierarchyMembers(context.Configuration, type);

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

   protected virtual ITypeRegistrationAction CreateTypeRegistrationActions(
     IEngineConfigurationProviderLoaderContext context) => new ApplyTypeConventionsAction(context.ConventionProvider) {
        NextAction = new ApplyTypeFactoryAction(context.ConfigurationProvider) {
           NextAction = new RegisterTypeMembersFromConfigurationAction(context.ConfigurationProvider) {
              NextAction = new ApplyTypeMemberConventionsAction(context.Configuration, context.ConventionProvider) {
                 NextAction = new ApplyTypeMemberConfigurationAction(context.ConfigurationProvider) {
                    NextAction = new CascadeBaseTypeConfigurationAction(context.Configuration) { NextAction = null }
                 }
              }
           }
        }
     };
}