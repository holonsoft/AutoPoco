using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

/// <param name="config">the finished engine configuration</param>
/// <param name="conventionProvider">conventions for types requested without configuration</param>
/// <param name="nullableAnnotations">optional, see <see cref="NullableAnnotationSettings" /></param>
/// <param name="seed">seed of every session created without an explicit one</param>
public class GenerationSessionFactory(IEngineConfiguration config, IEngineConventionProvider conventionProvider,
  NullableAnnotationSettings? nullableAnnotations = null, int seed = AutoPocoDefaults.Seed) : IGenerationSessionFactory {

   /// <summary>
   ///   The seed of this factory.
   /// </summary>
   public int Seed { get; } = seed;

   public IGenerationSession CreateSession(int recursionLimit)
      => CreateSession(recursionLimit, Seed);

   public IGenerationSession CreateSession(int recursionLimit, int seed)
      => new GenerationContext(new GenerationConfiguration(config, conventionProvider, recursionLimit, nullableAnnotations, seed));

   public IGenerationSession CreateSession() =>
      // TODO: Need to deep-clone the config
      CreateSession(AutoPocoDefaults.RecursionLimit);
}
