using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class TypeConventionContext(IEngineConfigurationType type) : ITypeConventionContext {
   public Type Target => type.RegisteredType;

   public void SetFactory(Type factory) {
      ArgumentNullException.ThrowIfNull(factory);
      type.SetFactory(new AutoPocoDataSourceFactory(factory));
   }

   public void SetFactory(Type factory, params object[] ctorArgs) {
      ArgumentNullException.ThrowIfNull(factory);
      ArgumentNullException.ThrowIfNull(ctorArgs);
      var sourceFactory = new AutoPocoDataSourceFactory(factory);
      sourceFactory.SetParams(ctorArgs);
      type.SetFactory(sourceFactory);
   }

   public void RegisterField(FieldInfo field) {
      ArgumentNullException.ThrowIfNull(field);
      var member = ReflectionHelper.GetMember(field);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(member);
   }

   public void RegisterProperty(PropertyInfo property) {
      ArgumentNullException.ThrowIfNull(property);
      var member = ReflectionHelper.GetMember(property);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(member);
   }

   public void RegisterMethod(MethodInfo method, MethodInvocationContext context) {
      ArgumentNullException.ThrowIfNull(method);
      ArgumentNullException.ThrowIfNull(context);
      var member = ReflectionHelper.GetMember(method);
      if (type.GetRegisteredMember(member) == null)
         type.RegisterMember(member);
      var registeredMember = type.GetRegisteredMember(member);
      registeredMember.SetDataSources(context.GetArguments());
   }
}
