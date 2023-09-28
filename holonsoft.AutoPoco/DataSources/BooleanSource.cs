using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;
public class BooleanSource : DataSourceBase<bool> {
   public override bool Next(IGenerationContext? context)
      => Random.Next(2) == 1;
}
