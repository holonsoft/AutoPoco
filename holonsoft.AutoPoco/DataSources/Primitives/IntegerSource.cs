using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public class IntegerSource(int min, int max) : DataSourceBase<int>
{
    public IntegerSource()
       : this(int.MinValue, int.MaxValue) { }

    public override int Next(IGenerationContext? context) => Random.Next(min, max);
}