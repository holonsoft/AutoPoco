using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class RootGenerationContextNode : IGenerationContextNode {
   public IGenerationContextNode? Parent => null;

   public GenerationTargetTypes ContextType => GenerationTargetTypes.Root;
}