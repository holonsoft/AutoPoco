using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectFieldSetFromValueAction(EngineTypeFieldMember member, object value) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) 
      => member.FieldInfo.SetValue(target, value);
}