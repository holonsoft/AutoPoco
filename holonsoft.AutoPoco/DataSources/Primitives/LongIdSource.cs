using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public class LongIdSource(long startValue = 0) : DataSourceBase<long> {

   private long _currentId = startValue;

   public LongIdSource() : this(0) { }

   public LongIdSource SetStartValue(long startValue) {
      _currentId = startValue;
      return this;
   }

   protected override long GetNextValue(IGenerationContext? context)
      => _currentId++;
}