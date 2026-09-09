using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

/// <summary>
///   A generator with its position in the collection it belongs to. The position survives <c>Random</c>.
/// </summary>
public sealed record IndexedGenerator<TPoco>(int Index, IObjectGenerator<TPoco> Generator);

/// <param name="generators">one generator per element, in collection order</param>
/// <param name="seed">seed of the shuffle behind <see cref="Random(int)" />, the session seed when created by a session</param>
public class CollectionContext<TPoco, TCollection> : ICollectionContext<TPoco, TCollection> where TCollection : ICollection<TPoco> {
   private readonly IndexedGenerator<TPoco>[] _generators;
   private readonly Random _random;

   public CollectionContext(IEnumerable<IObjectGenerator<TPoco>> generators, int seed = AutoPocoDefaults.Seed) {
      ArgumentNullException.ThrowIfNull(generators);
      _generators = generators.Select((g, i) => new IndexedGenerator<TPoco>(i, g)).ToArray();
      _random = new StableRandom(seed);
   }

   public ICollectionContext<TPoco, TCollection> Impose<TMember>(Expression<Func<TPoco, TMember>> propertyExpr, TMember value) {
      foreach (var item in _generators)
         item.Generator.Impose(propertyExpr, value);
      return this;
   }

   public ICollectionContext<TPoco, TCollection> Impose<TMember>(Expression<Func<TPoco, TMember>> propertyExpr, Func<int, TMember> valueFactory) {
      ArgumentNullException.ThrowIfNull(valueFactory);
      foreach (var item in _generators)
         item.Generator.Impose(propertyExpr, valueFactory(item.Index));
      return this;
   }

   public ICollectionContext<TPoco, TCollection> Impose<TMember>(Expression<Func<TPoco, TMember>> propertyExpr, Func<int, TPoco, TMember> valueFactory) {
      ArgumentNullException.ThrowIfNull(valueFactory);
      foreach (var item in _generators) {
         var index = item.Index;
         item.Generator.Impose(propertyExpr, poco => valueFactory(index, poco));
      }

      return this;
   }

   public ICollectionContext<TPoco, TCollection> Source<TMember>(Expression<Func<TPoco, TMember>> propertyExpr,
     IDataSource dataSource) {
      foreach (var item in _generators)
         item.Generator.Source(propertyExpr, dataSource);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> First(int count)
      => new CollectionSequenceSelectionContext<TPoco, TCollection>(this, _generators, count);

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Random(int count)
      // Randomize and return, the original positions travel with the generators
      => new CollectionSequenceSelectionContext<TPoco, TCollection>(
        this,
        _generators.OrderBy(r => _random.Next()).ToArray(),
        count);

   public TCollection Get() {
      // Create an array if it's an array
      if (typeof(TPoco[]).IsAssignableFrom(typeof(TCollection)))
         return (TCollection) (object) _generators.Select(x => x.Generator.Get()).ToArray();
      // Return a list if it's a list
      if (typeof(IList<>).MakeGenericType(typeof(TPoco)).IsAssignableFrom(typeof(TCollection)))
         return (TCollection) (object) _generators.Select(x => x.Generator.Get()).ToList();
      throw new InvalidOperationException();
   }

   public ICollectionContext<TPoco, TCollection> Invoke(Expression<Action<TPoco>> methodExpr) {
      foreach (var item in _generators)
         item.Generator.Invoke(methodExpr);
      return this;
   }

   public ICollectionContext<TPoco, TCollection> Invoke<TMember>(Expression<Func<TPoco, TMember>> methodExpr) {
      foreach (var item in _generators)
         item.Generator.Invoke(methodExpr);
      return this;
   }
}
