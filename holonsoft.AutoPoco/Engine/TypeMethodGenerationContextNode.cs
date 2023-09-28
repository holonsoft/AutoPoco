using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class TypeMethodGenerationContextNode(TypeGenerationContextNode parent, EngineTypeMethodMember method) : IGenerationContextNode {
   public virtual EngineTypeMethodMember Method { get; } = method;

   public virtual object Target => parent.Target ?? throw new InvalidOperationException();

   public virtual IGenerationContextNode Parent => parent;

   public GenerationTargetTypes ContextType => GenerationTargetTypes.Method;
}