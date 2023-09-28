using System.Reflection;

namespace holonsoft.AutoPoco.Configuration;

public class EngineTypeFieldMember(FieldInfo fieldInfo) : EngineTypeMember {
   public override string Name => FieldInfo.Name;

   public override bool IsMethod => false;

   public override bool IsField => true;

   public override bool IsProperty => false;

   public FieldInfo FieldInfo { get; } = fieldInfo;

   public override bool Equals(object? obj) {
      var otherMember = obj as EngineTypeFieldMember;
      if (otherMember == null) return false;

      return otherMember.FieldInfo == FieldInfo;
   }

   public override int GetHashCode() => FieldInfo.GetHashCode();
}