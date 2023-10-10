using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

public class AutoSource<T> : DataSourceBase<T>
{
    public override T Next(IGenerationContext? context)
       => context!.Single<T>().Get();
}