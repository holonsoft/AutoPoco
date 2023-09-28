using System.Reflection;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface ITypeConventionContext {
   /// <summary>
   ///   Gets the type being registered
   /// </summary>
   Type Target { get; }

   /// <summary>
   ///   Sets the factory for instantiating this type
   /// </summary>
   /// <param name="factory"></param>
   void SetFactory(Type factory);

   /// <summary>
   ///   Sets the factory for instantiating this type along with arguments for that factory
   /// </summary>
   void SetFactory(Type factory, params object[] ctorArgs);

   /// <summary>
   ///   Registers a field for auto-population
   /// </summary>
   void RegisterField(FieldInfo field);

   /// <summary>
   ///   Registers a property for auto-population
   /// </summary>
   void RegisterProperty(PropertyInfo property);

   /// <summary>
   ///   Registers a method for invocation along with the args to invoke it
   /// </summary>
   /// <remarks>If the arg is a Type and that type derives from IDataSource, then the data source will be created and invoked</remarks>
   void RegisterMethod(MethodInfo method, MethodInvocationContext context);
}