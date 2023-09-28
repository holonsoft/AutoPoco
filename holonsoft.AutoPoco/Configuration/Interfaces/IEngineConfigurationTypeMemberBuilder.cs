namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeMemberBuilder {
   /// <summary>
   ///   Uses the specified data source for member values
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder Use(Type dataSource);

   /// <summary>
   ///   Uses the specified data source for member values (with args)
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder Use(Type dataSource, params object[] args);

   /// <summary>
   ///   Allows this property to be set by convention
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder Default();
}