using System.Linq.Expressions;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class CollectionSequenceSelectionContext<TPoco, TCollection>
  : ICollectionSequenceSelectionContext<TPoco, TCollection> where TCollection : ICollection<TPoco> {
   private readonly IEnumerable<IObjectGenerator<TPoco>> _allGenerators;
   private int _currentCount;
   private int _currentSkip;
   private readonly ICollectionContext<TPoco, TCollection> _parentContext;
   private IEnumerable<IObjectGenerator<TPoco>>? _selected = null;

   public CollectionSequenceSelectionContext(
     ICollectionContext<TPoco, TCollection> parentContext,
     IEnumerable<IObjectGenerator<TPoco>> generators,
     int initialPull) {
      _allGenerators = generators;
      _currentCount = 0;
      _currentSkip = 0;
      _parentContext = parentContext;
      Next(initialPull);
   }

   public int Remaining => _allGenerators.Count() - (_currentSkip + _currentCount);

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Impose<TMember>(
     Expression<Func<TPoco, TMember>> propertyExpr, TMember value) {

      if (_selected == null)
         throw new Exception(@"Impose has no values for _selected!");

      foreach (var item in _selected)
         item.Impose(propertyExpr, value);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Invoke(Expression<Action<TPoco>> methodExpr) {
      if (_selected == null)
         throw new Exception(@"Invoke(Expression<Action<TPoco>>) has no values for _selected!");

      foreach (var item in _selected)
         item.Invoke(methodExpr);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Invoke<TMember>(
     Expression<Func<TPoco, TMember>> methodExpr) {
      if (_selected == null)
         throw new Exception(@"Invoke<TMember> has no values for _selected!");

      foreach (var item in _selected)
         item.Invoke(methodExpr);
      return this;
   }

   public ICollectionSequenceSelectionContext<TPoco, TCollection> Next(int count) {
      // Skip ahead + return this
      _currentSkip += _currentCount;
      _currentCount = count;
      _selected = _allGenerators
        .Skip(_currentSkip)
        .Take(_currentCount);

      return this;
   }

   public ICollectionContext<TPoco, TCollection> All() 
      => _parentContext;
}