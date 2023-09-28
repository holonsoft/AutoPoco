namespace holonsoft.AutoPoco.Configuration.Interfaces;

/// <summary>
///   Defines a registered member as part of a type
/// </summary>
public interface IEngineConfigurationTypeMember {
   /// <summary>
   ///   Gets the member this configuration is a part of
   /// </summary>
   EngineTypeMember Member { get; }

   /// <summary>
   ///   Sets a single data source for the type member
   /// </summary>
   /// <param name="action"></param>
   void SetDataSource(IEngineConfigurationDataSource action);

   /// <summary>
   ///   Sets multiple data sources for the type member
   /// </summary>
   /// <param name="sources"></param>
   void SetDataSources(IEnumerable<IEngineConfigurationDataSource> sources);

   /// <summary>
   ///   Gets the data sources required to populate this member on type instantiation
   /// </summary>
   /// <returns></returns>
   IEnumerable<IEngineConfigurationDataSource> GetDataSources();
}