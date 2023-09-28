using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public abstract class TypeRegistrationAction : ITypeRegistrationAction {
   public ITypeRegistrationAction? NextAction { get; set; }

   void ITypeRegistrationAction.Apply(IEngineConfigurationType type) {
      Apply(type);
      NextAction?.Apply(type);
   }

   public abstract void Apply(IEngineConfigurationType type);
}