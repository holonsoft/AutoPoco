using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class IntegerSourceBase<T>(int min, int max) : DataSourceBase<T> {
   public int Min { get; private set; } = min;
   public int Max { get; private set; } = max;

   public IntegerSourceBase<T> SetMinMax(int min, int max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.Next(Min, Max);
}

/// <summary>
/// Create an integer source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
public class IntegerSource(int min, int max) : IntegerSourceBase<int>(min, max) {
   public IntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

/// <summary>
/// Create a nullable integer source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableIntegerSource(int min, int max) : IntegerSourceBase<int?>(min, max) {
   public NullableIntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

