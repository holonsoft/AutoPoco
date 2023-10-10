using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class ApplyTypeFactoryAction(IEngineConfigurationProvider configurationProvider) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type) {
      var typeProvider =
         configurationProvider
            .GetConfigurationTypes()
            .FirstOrDefault(x => x.GetConfigurationType() == type.RegisteredType);

      if (typeProvider?.GetFactory() != null)
         type.SetFactory(typeProvider.GetFactory() 
                         ?? throw new InvalidOperationException());
      else if (type.GetFactory() == null) {
         // Activator.CreateInstance as a last resort
         var fallbackType = typeof(DefaultSource<>).MakeGenericType(type.RegisteredType);
         type.SetFactory(new DataSourceFactory(fallbackType));
      }
   }
}