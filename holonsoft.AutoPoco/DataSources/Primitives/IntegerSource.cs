using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class IntegerSourceBase<T>(int min, int max) : DataSourceBase<T> {
   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public int Min { get; private set; } = min;

   /// <summary>
   ///   Upper bound, inclusive.
   /// </summary>
   public int Max { get; private set; } = max;

   public IntegerSourceBase<T> SetMinMax(int min, int max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context)
      => (T) (object) Random.NextInclusive(Min, Max);
}

/// <summary>
/// Create an integer source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
public class IntegerSource(int min, int max) : IntegerSourceBase<int>(min, max) {
   public IntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

/// <summary>
/// Create a nullable integer source
/// </summary>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive</param>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults"/>
public class NullableIntegerSource(int min, int max) : IntegerSourceBase<int?>(min, max) {
   public NullableIntegerSource()
      : this(int.MinValue, int.MaxValue) { }
}

