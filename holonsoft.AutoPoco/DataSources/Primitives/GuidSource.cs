using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public class GuidSource : DataSourceBase<Guid>
{
    public override Guid Next(IGenerationContext? context)
    {
        var buffer = new byte[16];
        Random.NextBytes(buffer);

        // do not use Guid.NewGuid(), otherwise you do not get reproducable test data
        return new Guid(buffer);
    }
}