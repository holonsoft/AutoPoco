using System.Reflection;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Answers whether a member may hold null, based on nullable reference type annotations
///   and <see cref="Nullable{T}" />. Members compiled without nullable annotations (oblivious) count as not nullable.
/// </summary>
public static class NullabilityHelper {
   public static bool AllowsNull(PropertyInfo propertyInfo) {
      ArgumentNullException.ThrowIfNull(propertyInfo);
      return AllowsNull(propertyInfo.PropertyType, () => new NullabilityInfoContext().Create(propertyInfo));
   }

   public static bool AllowsNull(FieldInfo fieldInfo) {
      ArgumentNullException.ThrowIfNull(fieldInfo);
      return AllowsNull(fieldInfo.FieldType, () => new NullabilityInfoContext().Create(fieldInfo));
   }

   public static bool AllowsNull(ParameterInfo parameterInfo) {
      ArgumentNullException.ThrowIfNull(parameterInfo);
      return AllowsNull(parameterInfo.ParameterType, () => new NullabilityInfoContext().Create(parameterInfo));
   }

   private static bool AllowsNull(Type type, Func<NullabilityInfo> nullabilityInfo) {
      if (type.IsValueType)
         return Nullable.GetUnderlyingType(type) != null;

      var info = nullabilityInfo();

      // The engine writes into the member, so the write state decides. Get-only members have no write state.
      var state = info.WriteState != NullabilityState.Unknown
         ? info.WriteState
         : info.ReadState;

      return state == NullabilityState.Nullable;
   }
}
