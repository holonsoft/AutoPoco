using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class BooleanSourceBase<T> : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) (Random.Next(2) == 1);
}

/// <summary>
/// Create a source for boolean values
/// </summary>
public class BooleanSource : BooleanSourceBase<bool> { }

/// <summary>
/// Create a source for nullable boolean values. 
/// </summary>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableBooleanSource : BooleanSourceBase<bool?> { }
