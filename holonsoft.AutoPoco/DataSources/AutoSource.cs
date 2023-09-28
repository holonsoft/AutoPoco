using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;

public class AutoSource<T> : DataSourceBase<T> {
   public override T Next(IGenerationContext? context)
      => context!.Single<T>().Get();
}