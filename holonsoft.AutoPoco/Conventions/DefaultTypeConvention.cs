using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultTypeConvention : ITypeConvention {
   public void Apply(ITypeConventionContext context) {
      // Register every public property on this type
      foreach (var property in context.Target
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(x => !x.PropertyType.ContainsGenericParameters && IsDefinedOnType(x, context.Target)))
         if (PropertyHasPublicSetter(property))
            context.RegisterProperty(property);

      // Register every public field on this type
      foreach (var field in context.Target
                 .GetFields(BindingFlags.Public | BindingFlags.Instance)
                 .Where(x => !x.FieldType.ContainsGenericParameters && IsDefinedOnType(x, context.Target)))
         context.RegisterField(field);
   }

   private static bool PropertyHasPublicSetter(PropertyInfo property) {
      var setter = property.GetSetMethod();
      return setter != null && setter.IsPublic;
   }

   private static bool IsDefinedOnType(MemberInfo member, Type type) {
      if (member.DeclaringType != type)
         return false;

      if (member.MemberType != MemberTypes.Property || type.IsInterface) 
         return true;

      var property = (PropertyInfo) member;
      
      //FIXIT
      var interfaceMethods =
         from i in type.GetInterfaces()
         from method in type.GetInterfaceMap(i).TargetMethods
         select method;
      
      var exists = (from method in interfaceMethods
         where property.GetAccessors().Contains(method)
         select 1).Count() > 0;

      return !exists;
   }
}