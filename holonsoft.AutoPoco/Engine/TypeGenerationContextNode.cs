using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class TypeGenerationContextNode(IGenerationContextNode? parent, object? target) : IGenerationContextNode {
   public virtual object? Target { get; } = target;

   public virtual IGenerationContextNode? Parent { get; } = parent;

   public GenerationTargetTypes ContextType => GenerationTargetTypes.Object;
}