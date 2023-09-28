namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeBuilder {
   /// <summary>
   ///   Adds a specific rule for a property on the poco we're building rules for
   /// </summary>
   IEngineConfigurationTypeMemberBuilder SetupProperty(string propertyName);

   /// <summary>
   ///   Adds a specific rule for a field on the poco we're building rules for
   /// </summary>
   IEngineConfigurationTypeMemberBuilder SetupField(string fieldName);

   /// <summary>
   ///   Adds a rule for a method
   /// </summary>
   /// <param name="methodName">The method to be invoked on creation</param>
   /// <param name="context"></param>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder SetupMethod(string methodName, MethodInvocationContext context);

   /// <summary>
   ///   Sets the factory type for this builder
   /// </summary>
   IEngineConfigurationTypeBuilder ConstructWith(Type type);

   /// <summary>
   ///   Sets the factory type and arguments for that factory type
   /// </summary>
   IEngineConfigurationTypeBuilder ConstructWith(Type type, params object[] args);
}