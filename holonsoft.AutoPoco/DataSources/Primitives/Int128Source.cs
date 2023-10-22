using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

#if NET7_0_OR_GREATER
public abstract class Int128SourceBase<T>(Int128 min, Int128 max) : DataSourceBase<T> {
   public Int128 Min { get; private set; } = min;
   public Int128 Max { get; private set; } = max;

   public Int128SourceBase<T> SetMinMax(Int128 min, Int128 max) {
      Min = min;
      Max = max;
      return this;
   }

   protected override T GetNextValue(IGenerationContext? context) {
      var x = new Int128((ulong) Random.NextInt64(0, long.MaxValue), (ulong) Random.NextInt64(0, long.MaxValue));

      if (x < Min)
         x = Min;

      if (x > Max)
         x = Max;

      return (T) (object) x;
   }
}

/// <summary>
/// Create an Int128 source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
public class Int128Source(Int128 min, Int128 max) : Int128SourceBase<Int128>(min, max) {
   public Int128Source()
      : this(Int128.MinValue, Int128.MaxValue) { }
}

/// <summary>
/// Create a nullable Int128 source
/// </summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableInt128Source(Int128 min, Int128 max) : Int128SourceBase<Int128?>(min, max) {
   public NullableInt128Source()
      : this(Int128.MinValue, Int128.MaxValue) { }
}

#endif