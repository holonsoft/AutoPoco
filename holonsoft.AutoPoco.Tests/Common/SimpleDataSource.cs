using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleDataSource(string value) : DataSourceBase<string> {
   protected override string GetNextValue(IGenerationContext? context) => value;
}