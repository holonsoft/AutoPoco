using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectMethodInvokeFuncAction<TPoco, TReturn>(Func<TPoco, TReturn> action) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) 
      => action.Invoke((TPoco) target);
}