using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Conventions;

/// <summary>
///   Creates complex types through <see cref="CtorSource{T}" />, which picks the constructor once the
///   registered members are known and feeds matching parameters from their data sources.
/// </summary>
public class DefaultComplexTypeCtorConvention : ITypeConvention {
   public void Apply(ITypeConventionContext context) {
      var type = context.Target;
      if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
         return;

      if (ConstructorResolver.GetPublicConstructors(type).Length == 0)
         return;

      context.SetFactory(typeof(CtorSource<>).MakeGenericType(type));
   }
}
