namespace holonsoft.AutoPoco.Configuration;

public class TypeFieldConventionRequirements : TypeMemberConventionRequirements {
   public bool IsValid(EngineTypeFieldMember member) 
      => IsValidName(member.Name) 
         && IsValidType(member.FieldInfo.FieldType);
}