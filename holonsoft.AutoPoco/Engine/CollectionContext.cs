using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

/// <param name="generators">one generator per element</param>
/// <param name="seed">seed of the shuffle behind <see cref="Random(int)" />, the session seed when created by a session</param>
public class CollectionContext<TPoco, TCollection>(IEnumerable<IObjectGenerator<TPoco>> generators, int seed = AutoPocoDefaults.Seed)
  : ICollectionContext<TPoco, TCollection> where TCollection : ICollection<TPoco> {
   private readonly Random _random = new StableRandom(seed);

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