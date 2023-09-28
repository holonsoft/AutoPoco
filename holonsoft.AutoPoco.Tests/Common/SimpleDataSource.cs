using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleDataSource(string value) : DataSourceBase<string> {
   public override string Next(IGenerationContext? context) => value;
}