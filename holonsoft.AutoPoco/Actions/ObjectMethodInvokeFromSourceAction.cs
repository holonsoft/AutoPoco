using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

public class ObjectMethodInvokeFromSourceAction(EngineTypeMethodMember member, IEnumerable<IDataSource> sources) : IObjectAction {
   private readonly IEnumerable<IDataSource> _sources = sources.ToArray();

   public void Enact(IGenerationContext? context, object target) {
      var methodContext = new GenerationContext(context?.Builders!,
        new TypeMethodGenerationContextNode(context?.Node as TypeGenerationContextNode
                                            ?? throw new InvalidOperationException(), member));

      member.MethodInfo.Invoke(target, _sources.Select(source => source.InternalNext(methodContext)).ToArray());
   }
}