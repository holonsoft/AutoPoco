using holonsoft.AutoPoco.Engine.Enums;

namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IGenerationContextNode {
   /// <summary>
   ///   Gets the next level up in the call graph
   /// </summary>
   IGenerationContextNode? Parent { get; }

   GenerationTargetTypes ContextType { get; }
}