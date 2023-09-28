using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class TypeFieldGenerationContextNode(TypeGenerationContextNode parent, EngineTypeFieldMember field) : IGenerationContextNode {
   public virtual EngineTypeFieldMember Field { get; } = field;

   public virtual object Target => parent.Target ?? throw new InvalidOperationException();

   public virtual IGenerationContextNode Parent => parent;

   public GenerationTargetTypes ContextType => GenerationTargetTypes.Field;
}