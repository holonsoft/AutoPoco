namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IGenerationSessionFactory {
   /// <summary>
   ///   Creates a session from this configured factory
   /// </summary>
   /// <returns></returns>
   IGenerationSession CreateSession();

   /// <summary>
   ///   Creates a session, overriding the default recursion limit
   /// </summary>
   IGenerationSession CreateSession(int recursionLimit);

   /// <summary>
   ///   Creates a session with its own seed instead of the seed of the factory. Two sessions with the same
   ///   seed produce the same data, the seed alone defines every sequence.
   /// </summary>
   IGenerationSession CreateSession(int recursionLimit, int seed);
}