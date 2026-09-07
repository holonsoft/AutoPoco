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

   /// <summary>
   ///   Lets nullable annotations drive null generation: every property or field declared as nullable
   ///   (<c>string?</c>, <c>int?</c>, ...) becomes null with the given probability, whatever its data source.
   ///   Off by default. Members without a nullable annotation and values set by <c>Impose</c> are never touched.
   /// </summary>
   /// <param name="nullCreationThreshold">probability in percent (0 to 100), default is <see cref="AutoPocoGlobalSettings.NullCreationThreshold" /></param>
   void RespectNullableAnnotations(int? nullCreationThreshold = null);
}