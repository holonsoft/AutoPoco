using System.Linq.Expressions;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface ITypeMemberConventionRequirements {
   /// <summary>
   ///   Defines a rule for this convention's application that depends on the name of this member
   /// </summary>
   /// <param name="rule"></param>
   void Name(Expression<Func<string, bool>> rule);

   /// <summary>
   ///   Defines a rule for this convention's application that depends on the type of this member
   /// </summary>
   /// <param name="rule"></param>
   void Type(Expression<Func<Type, bool>> rule);
}