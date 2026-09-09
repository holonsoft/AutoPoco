namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IGenerationConfiguration {
   /// <summary>
   ///   Gets the recursion limit for this configuration
   /// </summary>
   int RecursionLimit { get; }

   /// <summary>
   ///   Gets the settings that turn nullable annotations into random nulls
   /// </summary>
   NullableAnnotationSettings NullableAnnotations { get; }

   /// <summary>
   ///   Gets the seed of this session. Every data source gets its own stream derived from it.
   /// </summary>
   int Seed { get; }

   /// <summary>
   ///   Gets the object builder for a certain type
   /// </summary>
   /// <param name="searchType"></param>
   /// <returns></returns>
   IObjectBuilder GetBuilderForType(Type searchType);
}