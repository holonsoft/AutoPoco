using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Actions;

/// <summary>
///   Sets a property or field from a lambda that receives the object as generated so far, so the value
///   can depend on other members (or on the position in a collection, captured by the lambda).
/// </summary>
public class ObjectMemberSetFromFuncAction<TPoco, TMember> : IObjectAction {
   private readonly EngineTypeMember _member;
   private readonly Func<TPoco, TMember> _valueFactory;

   public ObjectMemberSetFromFuncAction(EngineTypeMember member, Func<TPoco, TMember> valueFactory) {
      ArgumentNullException.ThrowIfNull(member);
      ArgumentNullException.ThrowIfNull(valueFactory);
      if (member is not (EngineTypePropertyMember or EngineTypeFieldMember))
         throw new ArgumentException($"Member '{member.Name}' is neither a property nor a field.", nameof(member));

      _member = member;
      _valueFactory = valueFactory;
   }

   public void Enact(IGenerationContext? context, object target) {
      var value = _valueFactory((TPoco) target);

      switch (_member) {
         case EngineTypePropertyMember property:
            property.PropertyInfo.SetValue(target, value, null);
            break;
         case EngineTypeFieldMember field:
            field.FieldInfo.SetValue(target, value);
            break;
      }
   }
}
