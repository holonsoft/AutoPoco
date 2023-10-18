using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   Create an integer ID source, default start value is 0
/// </summary>
public class IntegerIdSource(int startValue = 0) : DataSourceBase<int> {
   private int _currentId = startValue;

   public IntegerIdSource() : this(0) { }

   public IntegerIdSource SetStartValue(int startValue) {
      _currentId = startValue;
      return this;
   }

   protected override int GetNextValue(IGenerationContext? context)
      => _currentId++;
}