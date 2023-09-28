namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface ITypeFieldConvention : IConvention {
   /// <summary>
   ///   Specify the conditions under which this convention will be invoked
   /// </summary>
   /// <param name="requirements"></param>
   void SpecifyRequirements(ITypeMemberConventionRequirements requirements);

   /// <summary>
   ///   Apply the convention to the registered field
   /// </summary>
   void Apply(ITypeFieldConventionContext context);
}