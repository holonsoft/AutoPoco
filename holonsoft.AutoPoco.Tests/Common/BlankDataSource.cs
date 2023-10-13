using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Common;

public class BlankDataSource : DataSourceBase<int> {
   protected override int GetNextValue(IGenerationContext? context) => 0;
}