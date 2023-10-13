using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectFieldSetFromSourceAction(EngineTypeFieldMember member, IDataSource source) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) {
      var fieldContext = new GenerationContext(context?.Builders!, new TypeFieldGenerationContextNode(
        context?.Node as TypeGenerationContextNode ?? throw new InvalidOperationException(), member));

      member.FieldInfo.SetValue(target, source.InternalNext(fieldContext));
   }
}