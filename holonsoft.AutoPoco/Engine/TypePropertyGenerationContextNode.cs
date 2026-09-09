using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class TypePropertyGenerationContextNode(TypeGenerationContextNode parent, EngineTypePropertyMember property) : IGenerationContextNode {
   public virtual EngineTypePropertyMember Property { get; } = property;

   public virtual object Target => parent.Target ?? throw new InvalidOperationException("The object of this node does not exist yet, it is still being constructed.");

   public virtual IGenerationContextNode Parent => parent;

   public GenerationTargetTypes ContextType => GenerationTargetTypes.Property;
}