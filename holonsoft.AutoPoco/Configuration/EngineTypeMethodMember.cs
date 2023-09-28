using System.Reflection;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class EngineTypeMethodMember(MethodInfo methodInfo) : EngineTypeMember {
   public override string Name => MethodInfo.Name;

   public override bool IsMethod => true;

   public override bool IsField => false;

   public override bool IsProperty => false;

   public MethodInfo MethodInfo { get; } = methodInfo;

   public override bool Equals(object? obj) {
      var otherMember = obj as EngineTypeMethodMember;

      if (otherMember == null)
         return false;

      return otherMember.MethodInfo.Name == MethodInfo.Name 
            && otherMember.MethodInfo.ArgumentsAreEqualTo(MethodInfo);
   }

   public override int GetHashCode() => MethodInfo.GetHashCode();
}