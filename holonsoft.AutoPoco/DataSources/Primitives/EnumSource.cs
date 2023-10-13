using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public abstract class EnumSourceBase<T>(int? nullCreationThreshold = null) : DataSourceBase<T>
      where T : Enum {
   protected override T GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return default!;
      }

      var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
      return values[Random.Next(values.Length)];
   }
}

public class EnumSource<T> : EnumSourceBase<T>
   where T : struct, Enum { }

//public class NullableEnumSource<T> : EnumSourceBase<T>
//   where T : Enum {

//   public NullableEnumSource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }
//}