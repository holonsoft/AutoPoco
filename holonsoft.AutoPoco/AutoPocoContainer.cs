// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoPocoContainer.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco;

/// <summary>
///   The auto poco container.
/// </summary>
public static class AutoPocoContainer {
   /// <summary>
   ///   The configure.
   /// </summary>
   /// <param name="setup">
   ///   The setup.
   /// </param>
   /// <returns>
   ///   The <see cref="IGenerationSessionFactory" />.
   /// </returns>
   public static IGenerationSessionFactory Configure(Action<IEngineConfigurationBuilder> setup) {
      var config = new EngineConfigurationBuilder();
      config.Conventions(x => x.Register<DefaultPrimitiveCtorConvention>());
      setup.Invoke(config);
      var configFactory = new EngineConfigurationFactory();
      return new GenerationSessionFactory(
        configFactory.Create(config, config.ConventionProvider),
        config.ConventionProvider);
   }

   /// <summary>
   ///   The create default session.
   /// </summary>
   /// <returns>
   ///   The <see cref="IGenerationSession" />.
   /// </returns>
   public static IGenerationSession CreateDefaultSession() {
      var config = new EngineConfigurationBuilder();
      var configFactory = new EngineConfigurationFactory();

      config.Conventions(x => x.UseDefaultConventions());

      return
        new GenerationSessionFactory(
          configFactory.Create(config, config.ConventionProvider),
          config.ConventionProvider).CreateSession();
   }
}