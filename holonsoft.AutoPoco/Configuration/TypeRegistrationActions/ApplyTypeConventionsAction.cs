using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class ApplyTypeConventionsAction(IEngineConventionProvider conventions) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type)
      => conventions.Find<ITypeConvention>()
        .Select(t => (ITypeConvention) Activator.CreateInstance(t)!)
        .ToList()
        .ForEach(x => x.Apply(new TypeConventionContext(type)));
}