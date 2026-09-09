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
                         ?? throw new InvalidOperationException($"The configured factory of type '{type.RegisteredType.FullName}' is null."));
      else if (type.GetFactory() == null)
         type.SetFactory(new AutoPocoDataSourceFactory(FallbackFactoryType(type.RegisteredType)));
   }

   /// <summary>
   ///   Concrete classes are created through their constructor, fed from the registered members
   ///   (records and immutable types included). Everything else falls back to the default value or Activator.
   /// </summary>
   private static Type FallbackFactoryType(Type type)
      => type.IsClass && !type.IsAbstract && type != typeof(string)
         ? typeof(CtorSource<>).MakeGenericType(type)
         : typeof(DefaultSource<>).MakeGenericType(type);
}
