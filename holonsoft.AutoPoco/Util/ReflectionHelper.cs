using System.Linq.Expressions;
using System.Reflection;
using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Util;

public static class ReflectionHelper {
   public static bool LooseCompare(this MemberInfo info, MemberInfo other) 
      => info.MetadataToken == other.MetadataToken && info.Module == other.Module;

   public static bool ArgumentsAreEqualTo(this MethodInfo one, MethodInfo two) {
      var paramOne = one.GetParameters();
      var paramTwo = two.GetParameters();

      if (paramTwo.Length != paramOne.Length)
         return false;

      return !paramOne.Where((t, x) 
         => t != paramTwo[x]).Any();
   }

   public static EngineTypeMember GetMember<TPoco, TReturn>(Expression<Func<TPoco, TReturn>> expression) {
      var member = GetMemberInfo(typeof(TPoco), expression.Body);
      return GetMember(member);
   }

   public static EngineTypeMember GetMember<TPoco>(Expression<Func<TPoco, object>> expression) {
      var member = GetMemberInfo(typeof(TPoco), expression.Body);
      return GetMember(member);
   }

   public static PropertyInfo GetProperty<TPoco>(Expression<Func<TPoco, object>> expression) {
      var member = GetMemberInfo(typeof(TPoco), expression.Body);
      return member.ReflectedType!.GetProperty(member.Name)!;
   }

   public static FieldInfo GetField<TPoco>(Expression<Func<TPoco, object>> expression) {
      var member = GetMemberInfo(typeof(TPoco), expression.Body);
      return member.ReflectedType!.GetField(member.Name)!;
   }

#pragma warning disable IDE0060 // Remove unused parameter
   private static MemberInfo GetMemberInfo(Type declaringType, Expression expression) {
      if (expression is not MemberExpression memberExpression)
         throw new ArgumentException(@$"Expression not supported for '{nameof(declaringType)}'", nameof(expression));

      return memberExpression.Member;
   }
#pragma warning restore IDE0060 // Remove unused parameter

   public static string GetMethodName<TPoco>(Expression<Action<TPoco>> action) {
      if (action.Body is not MethodCallExpression methodExpression)
         throw new ArgumentException(@"Method expression expected, and not passed in", nameof(action));
      return methodExpression.Method.Name;
   }

   public static string GetMethodName<TPoco, TReturn>(Expression<Func<TPoco, TReturn>> function) {
      if (function.Body is not MethodCallExpression methodExpression)
         throw new ArgumentException(@"Method expression expected, and not passed in", nameof(function));
      return methodExpression.Method.Name;
   }

   public static EngineTypeMember GetMember(MemberInfo info) {
      return info switch {
         PropertyInfo propertyInfo => new EngineTypePropertyMember(propertyInfo),
         MethodInfo methodInfo => new EngineTypeMethodMember(methodInfo),
         FieldInfo fieldInfo => new EngineTypeFieldMember(fieldInfo),
         _ => throw new ArgumentException(@"Unsupported member type", nameof(info))
      };
   }
}