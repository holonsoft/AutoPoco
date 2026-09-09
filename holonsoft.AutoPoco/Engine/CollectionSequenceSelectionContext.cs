using System.Linq.Expressions;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class CollectionSequenceSelectionContext<TPoco, TCollection>
  : ICollectionSequenceSelectionContext<TPoco, TCollection> where TCollection : ICollection<TPoco> {
   private readonly IReadOnlyList<IndexedGenerator<TPoco>> _allGenerators;
   private int _currentCount;
   private int _currentSkip;
   private readonly ICollectionContext<TPoco, TCollection> _parentContext;
   private IReadOnlyList<IndexedGenerator<TPoco>> _selected = [];

   public CollectionSequenceSelectionContext(
     ICollectionContext<TPoco, TCollection> parentContext,
     IReadOnlyList<IndexedGenerator<TPoco>> generators,
     int initialPull) {
      ArgumentNullException.ThrowIfNull(parentContext);
      ArgumentNullException.ThrowIfNull(generators);
      _allGenerators = generators;
      _currentCount = 0;
      _currentSkip = 0;
      _parentContext = parentContext;
      Next(initialPull);
   }

   /// <summary>
   ///   Selection over generators without positions, they get their position from the enumeration order.
   /// </summary>
   public CollectionSequenceSelectionContext(
     ICollectionContext<TPoco, TCollection> parentContext,
     IEnumerable<IObjectGenerator<TPoco>> generators,
     int initialPull)
      : this(parentContext, (generators ?? throw new ArgumentNullException(nameof(generators))).Select((g, i) => new IndexedGenerator<TPoco>(i, g)).ToList(), initialPull) { }

   public int Remaining => _allGenerators.Count - (_currentSkip + _currentCount);

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Impose<TMember>(
     Expression<Func<TPoco, TMember>> propertyExpr, TMember value) {
      ArgumentNullException.ThrowIfNull(propertyExpr);
      foreach (var item in _selected)
         item.Generator.Impose(propertyExpr, value);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Impose<TMember>(
     Expression<Func<TPoco, TMember>> propertyExpr, Func<int, TMember> valueFactory) {
      ArgumentNullException.ThrowIfNull(propertyExpr);
      ArgumentNullException.ThrowIfNull(valueFactory);
      foreach (var item in _selected)
         item.Generator.Impose(propertyExpr, valueFactory(item.Index));
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Impose<TMember>(
     Expression<Func<TPoco, TMember>> propertyExpr, Func<int, TPoco, TMember> valueFactory) {
      ArgumentNullException.ThrowIfNull(propertyExpr);
      ArgumentNullException.ThrowIfNull(valueFactory);
      foreach (var item in _selected) {
         var index = item.Index;
         item.Generator.Impose(propertyExpr, poco => valueFactory(index, poco));
      }

      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Invoke(Expression<Action<TPoco>> methodExpr) {
      ArgumentNullException.ThrowIfNull(methodExpr);
      foreach (var item in _selected)
         item.Generator.Invoke(methodExpr);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Invoke<TMember>(
     Expression<Func<TPoco, TMember>> methodExpr) {
      ArgumentNullException.ThrowIfNull(methodExpr);
      foreach (var item in _selected)
         item.Generator.Invoke(methodExpr);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Next(int count) {
      ArgumentOutOfRangeException.ThrowIfNegative(count);
      // Skip ahead + return this
      _currentSkip += _currentCount;
      _currentCount = count;
      _selected = _allGenerators
        .Skip(_currentSkip)
        .Take(_currentCount)
        .ToList();

      return this;
   }

   public ICollectionContext<TPoco, TCollection> All()
      => _parentContext;
}
