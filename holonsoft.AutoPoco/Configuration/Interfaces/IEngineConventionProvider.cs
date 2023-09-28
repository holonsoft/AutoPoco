namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConventionProvider {
   /// <summary>
   ///   Finds all convention types matching the type passed in as the generic argument
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   IEnumerable<Type> Find<T>() where T : IConvention;
}