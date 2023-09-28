namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IGenerationSessionFactory {
   /// <summary>
   ///   Creates a session from this configured factory
   /// </summary>
   /// <returns></returns>
   IGenerationSession CreateSession();

   /// <summary>
   ///   Creates a session, overriding the default recursion limit
   ///   Note: This method signature will probably change at some point
   /// </summary>
   /// <returns></returns>
   IGenerationSession CreateSession(int recursionLimit);
}