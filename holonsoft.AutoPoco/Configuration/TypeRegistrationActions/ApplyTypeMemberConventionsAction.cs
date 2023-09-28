using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration.TypeRegistrationActions;

public class ApplyTypeMemberConventionsAction(IEngineConfiguration configuration, IEngineConventionProvider conventions) : TypeRegistrationAction {
   public override void Apply(IEngineConfigurationType type) => ApplyToType(type);

   private void ApplyToType(IEngineConfigurationType type) {
      foreach (var member in type.GetRegisteredMembers())
         ApplyToTypeMember(member);
   }

   private void ApplyToTypeMember(IEngineConfigurationTypeMember member) {
      if (member.Member.IsField)
         ApplyFieldConventions(member);
      if (member.Member.IsProperty)
         ApplyPropertyConventions(member);
   }

   private void ApplyPropertyConventions(IEngineConfigurationTypeMember member) {
      var convention = conventions.Find<ITypePropertyConvention>()
         .Select(t => {
            var details = new {
               Convention = (ITypePropertyConvention) Activator.CreateInstance(t)!,
               Requirements = new TypePropertyConventionRequirements()
            };
            details.Convention.SpecifyRequirements(details.Requirements);
            return details;
         })
         .Where(x => x.Requirements.IsValid((EngineTypePropertyMember) member.Member))
         .MaxBy(x => ScoreRequirements(x.Requirements));

      convention?.Convention.Apply(new TypePropertyConventionContext(configuration, member));
   }

   private void ApplyFieldConventions(IEngineConfigurationTypeMember member) {
      var convention = conventions.Find<ITypeFieldConvention>()
         .Select(t => {
            var details = new {
               Convention = (ITypeFieldConvention) Activator.CreateInstance(t)!,
               Requirements = new TypeFieldConventionRequirements()
            };
            details.Convention.SpecifyRequirements(details.Requirements);
            return details;
         })
         .Where(x => x.Requirements.IsValid((EngineTypeFieldMember) member.Member))
         .MaxBy(x => ScoreRequirements(x.Requirements));

      convention?.Convention.Apply(new TypeFieldConventionContext(configuration, member));
   }

   private static int ScoreRequirements(TypeMemberConventionRequirements requirements) {
      var score = 0;
      if (requirements.HasNameRule())
         score += 2;
      if (requirements.HasTypeRule())
         score++;
      return score;
   }
}