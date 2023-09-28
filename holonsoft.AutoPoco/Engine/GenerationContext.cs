using holonsoft.AutoPoco.Engine.Enums;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class GenerationContext : IGenerationContext {
   public IGenerationContextNode? Node { get; }

   public int Depth { get; private set; }

   public IGenerationConfiguration Builders { get; }

   public GenerationContext(IGenerationConfiguration objectBuilders)
     : this(objectBuilders, null) {
   }

   public GenerationContext(IGenerationConfiguration objectBuilders, IGenerationContextNode? node) {
      Builders = objectBuilders;
      Node = node;
      CalculateDepth();
   }

   public virtual IObjectGenerator<TPoco> Single<TPoco>() {
      var searchType = typeof(TPoco);
      var foundType = Builders!.GetBuilderForType(searchType);
      return new ObjectGenerator<TPoco>(this, foundType);
   }

   public ICollectionContext<TPoco, IList<TPoco>> List<TPoco>(int count) => new CollectionContext<TPoco, IList<TPoco>>(
        Enumerable.Range(0, count)
          .Select(x => Single<TPoco>()).ToArray()
          .AsEnumerable());

   public TPoco Next<TPoco>() => Single<TPoco>().Get();

   public TPoco Next<TPoco>(Action<IObjectGenerator<TPoco>> cfg) {
      var generator = Single<TPoco>();
      cfg.Invoke(generator);
      return generator.Get();
   }

   public IEnumerable<TPoco> Collection<TPoco>(int count) {
      var generator = List<TPoco>(count);
      return generator.Get();
   }

   public IEnumerable<TPoco> Collection<TPoco>(int count, Action<ICollectionContext<TPoco, IList<TPoco>>> cfg) {
      var generator = List<TPoco>(count);
      cfg.Invoke(generator);
      return generator.Get();
   }

   private void CalculateDepth() {
      var currentNode = Node;
      var depth = 0;
      while (currentNode != null) {
         currentNode = FindNextTypeNode(currentNode);
         depth++;
      }

      Depth = depth;
   }

   private static IGenerationContextNode? FindNextTypeNode(IGenerationContextNode? currentNode) {
      while (true) {
         currentNode = currentNode?.Parent;
         if (currentNode == null || currentNode.ContextType == GenerationTargetTypes.Object)
            break;
      }

      return currentNode;
   }
}