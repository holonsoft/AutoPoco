using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class TypeMemberConventionRequirements : ITypeMemberConventionRequirements {
   private Func<string, bool>? _nameRule;
   private Func<Type, bool>? _typeRule;

   public void Name(Expression<Func<string, bool>> rule) 
      => _nameRule = rule.Compile();

   public void Type(Expression<Func<Type, bool>> rule) 
      => _typeRule = rule.Compile();

   public bool IsValidType(Type type) 
      => _typeRule == null || _typeRule.Invoke(type);

   public bool IsValidName(string name) 
      => _nameRule == null || _nameRule.Invoke(name);

   internal bool HasNameRule() 
      => _nameRule != null;

   internal bool HasTypeRule() 
      => _typeRule != null;
}