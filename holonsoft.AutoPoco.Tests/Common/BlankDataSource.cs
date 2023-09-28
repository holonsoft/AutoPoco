using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Common;

public class BlankDataSource : DataSourceBase<int> {
   public override int Next(IGenerationContext? context) => 0;
}