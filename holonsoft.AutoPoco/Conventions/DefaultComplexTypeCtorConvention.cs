using System.Linq;
using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultComplexTypeCtorConvention : ITypeConvention {
   public void Apply(ITypeConventionContext context) {
      var type = context.Target;
      if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
         return;

      var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                    .MinBy(x => x.GetParameters().Length);

      if (ctor == null)
         return;

      var ctorSourceType = typeof(CtorSource<>).MakeGenericType(type);

      context.SetFactory(ctorSourceType, ctor);
   }
}