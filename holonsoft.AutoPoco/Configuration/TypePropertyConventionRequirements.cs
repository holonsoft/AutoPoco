namespace holonsoft.AutoPoco.Configuration;

public class TypePropertyConventionRequirements : TypeMemberConventionRequirements {
   public bool IsValid(EngineTypePropertyMember member) 
      => IsValidName(member.Name) 
         && IsValidType(member.PropertyInfo.PropertyType);
}