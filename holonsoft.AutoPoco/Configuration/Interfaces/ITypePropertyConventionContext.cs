using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface ITypePropertyConventionContext {
   /// <summary>
   ///   Gets the configuration created so far
   /// </summary>
   IEngineConfiguration Configuration { get; }

   /// <summary>
   ///   Gets the member being processed
   /// </summary>
   EngineTypePropertyMember Member { get; }

   /// <summary>
   ///   Sets the value of the member directly on instantiation
   /// </summary>
   /// <param name="value"></param>
   void SetValue(object value);

   /// <summary>
   ///   Sets the data source where this member will get data from
   /// </summary>
   /// <typeparam name="T"></typeparam>
   void SetSource<T>() where T : IDataSource;

   /// <summary>
   ///   Sets the data source where this member will get data from
   /// </summary>
   void SetSource(Type type);
}