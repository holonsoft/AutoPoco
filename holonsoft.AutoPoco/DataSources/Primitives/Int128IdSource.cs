using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

#if NET7_0_OR_GREATER
public class Int128IdSource(Int128 startValue) : DataSourceBase<Int128> {

   private Int128 _currentId = startValue;

   public Int128IdSource() : this(Int128.Zero) { }

   public Int128IdSource SetStartValue(long startValue) {
      _currentId = startValue;
      return this;
   }

   protected override Int128 GetNextValue(IGenerationContext? context)
      => _currentId++;
}

#endif