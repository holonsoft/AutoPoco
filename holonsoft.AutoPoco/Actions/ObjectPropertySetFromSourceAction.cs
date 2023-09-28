using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectPropertySetFromSourceAction(EngineTypePropertyMember member, IDataSource source) : IObjectAction {
   public void Enact(IGenerationContext? context, object target) {
      var propertyContext = new GenerationContext(context?.Builders!,
                              new TypePropertyGenerationContextNode(context?.Node as TypeGenerationContextNode ?? throw new InvalidOperationException(), member));
      member.PropertyInfo.SetValue(target, source.Next(propertyContext), null);
   }
}