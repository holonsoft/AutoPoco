using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Conventions;

public class DefaultPrimitiveCtorConvention : ITypeConvention {
   public void Apply(ITypeConventionContext context) {
      var type = context.Target;
      if (type.IsPrimitive || type == typeof(decimal))
         context.SetFactory(typeof(DefaultSource<>).MakeGenericType(type));
      else if (type == typeof(string))
         context.SetFactory(typeof(DefaultStringSource));
   }
}