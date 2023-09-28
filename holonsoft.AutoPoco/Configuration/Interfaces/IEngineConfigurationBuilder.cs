namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationBuilder {
   /// <summary>
   ///   Registers a type with the configuration and allows further configuration of that type
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Include<TPoco>();

   /// <summary>
   ///   Registers a type with the configuration and allows further configuration of that type
   /// </summary>
   /// <param name="t"></param>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder Include(Type t);

   /// <summary>
   ///   Sets up the conventions that the engine will use
   /// </summary>
   void Conventions(Action<IEngineConventionConfiguration> config);

   /// <summary>
   ///   Manually adds a type provider to the builder
   /// </summary>
   void RegisterTypeProvider(IEngineConfigurationTypeProvider provider);
}