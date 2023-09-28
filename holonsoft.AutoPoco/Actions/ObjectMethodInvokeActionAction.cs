using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectMethodInvokeActionAction<T>(Action<T> action) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) 
      => action.Invoke((T) target);
}