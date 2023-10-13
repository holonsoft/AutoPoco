using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class GuidSourceBase<T> : DataSourceBase<T> {
   protected override T GetNextValue(IGenerationContext? context) {
      var buffer = new byte[16];
      Random.NextBytes(buffer);

      // do not use Guid.NewGuid(), otherwise you do not get reproducable test data
      return (T) (object) new Guid(buffer);
   }
}

public class GuidSource : GuidSourceBase<Guid> { }

public class NullableGuidSource : GuidSourceBase<Guid?> { }