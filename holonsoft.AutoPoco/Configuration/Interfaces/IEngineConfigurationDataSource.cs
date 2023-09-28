using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationDataSource {
   /// <summary>
   ///   Builds the data source this configuration item represents
   /// </summary>
   /// <returns></returns>
   IDataSource? Build();
}