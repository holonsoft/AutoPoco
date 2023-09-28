﻿namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeProvider {
   /// <summary>
   ///   Gets the type this provider represents
   /// </summary>
   /// <returns></returns>
   Type GetConfigurationType();

   /// <summary>
   ///   Gets all of the members registered for this type
   /// </summary>
   /// <returns></returns>
   IEnumerable<IEngineConfigurationTypeMemberProvider> GetConfigurationMembers();

   /// <summary>
   ///   Gets the factory that will be used to construct objects
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationDataSource? GetFactory();
}