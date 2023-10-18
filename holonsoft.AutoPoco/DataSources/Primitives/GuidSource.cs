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

/// <summary>
/// Create a GUID source
/// </summary>
public class GuidSource : GuidSourceBase<Guid> { }

/// <summary>
/// Create a nullable GUID source
/// </summary>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableGuidSource : GuidSourceBase<Guid?> { }