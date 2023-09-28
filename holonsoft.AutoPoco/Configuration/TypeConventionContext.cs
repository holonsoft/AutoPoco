using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class TypeConventionContext(IEngineConfigurationType type) : ITypeConventionContext {
   public Type Target => type.RegisteredType;

   public void SetFactory(Type factory) => type.SetFactory(new DataSourceFactory(factory));

   public void SetFactory(Type factory, params object[] ctorArgs) {
      var sourceFactory = new DataSourceFactory(factory);
      sourceFactory.SetParams(ctorArgs);
      type.SetFactory(sourceFactory);
   }

   public void RegisterField(FieldInfo field) {
      var member = ReflectionHelper.GetMember(field);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(member);
   }

   public void RegisterProperty(PropertyInfo property) {
      var member = ReflectionHelper.GetMember(property);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(ReflectionHelper.GetMember(property));
   }

   public void RegisterMethod(MethodInfo method, MethodInvocationContext context) {
      var member = ReflectionHelper.GetMember(method);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(member);
      var registeredMember = type.GetRegisteredMember(member);
      registeredMember.SetDataSources(context.GetArguments());
   }
}