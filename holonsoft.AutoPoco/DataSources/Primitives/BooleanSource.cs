using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class BooleanSourceBase<T> : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) (Random.Next(2) == 1);
}

public class BooleanSource : BooleanSourceBase<bool> { }

public class NullableBooleanSource : BooleanSourceBase<bool?> { }

