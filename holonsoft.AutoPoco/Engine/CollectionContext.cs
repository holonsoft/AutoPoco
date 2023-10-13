using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class CollectionContext<TPoco, TCollection>(IEnumerable<IObjectGenerator<TPoco>> generators)
  : ICollectionContext<TPoco, TCollection> where TCollection : ICollection<TPoco> {
   private readonly Random _random = new(AutoPocoGlobalSettings.StandardSeed);

   public ICollectionContext<TPoco, TCollection> Impose<TMember>(Expression<Func<TPoco, TMember>> propertyExpr,
    TMember value) {
      foreach (var item in generators)
         item.Impose(propertyExpr, value);
      return this;
   }

   public ICollectionContext<TPoco, TCollection> Source<TMember>(Expression<Func<TPoco, TMember>> propertyExpr,
     IDataSource dataSource) {
      foreach (var item in generators)
         item.Source(propertyExpr, dataSource);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> First(int count) => new CollectionSequenceSelectionContext<TPoco, TCollection>(
        this,
        generators,
        count);

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Random(int count)
      // Randomize and return
      => new CollectionSequenceSelectionContext<TPoco, TCollection>(
        this,
        generators.OrderBy(r => _random.Next()).ToArray(),
        count);

   public TCollection Get() {
      // Create an array if it's an array
      if (typeof(TPoco[]).IsAssignableFrom(typeof(TCollection)))
         return (TCollection) (object) generators.Select(x => x.Get()).ToArray();
      // Return a list if it's a list
      if (typeof(IList<>).MakeGenericType(typeof(TPoco)).IsAssignableFrom(typeof(TCollection)))
         return (TCollection) (object) generators.Select(x => x.Get()).ToList();
      throw new InvalidOperationException();
   }

   public ICollectionContext<TPoco, TCollection> Invoke(Expression<Action<TPoco>> methodExpr) {
      foreach (var item in generators)
         item.Invoke(methodExpr);
      return this;
   }

   public ICollectionContext<TPoco, TCollection> Invoke<TMember>(Expression<Func<TPoco, TMember>> methodExpr) {
      foreach (var item in generators)
         item.Invoke(methodExpr);
      return this;
   }
}