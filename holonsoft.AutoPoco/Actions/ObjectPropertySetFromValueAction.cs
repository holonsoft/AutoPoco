using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectPropertySetFromValueAction(EngineTypePropertyMember member, object value) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) 
      => member.PropertyInfo.SetValue(target, value, null);
}