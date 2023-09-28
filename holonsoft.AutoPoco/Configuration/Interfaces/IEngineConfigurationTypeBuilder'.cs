using System.Linq.Expressions;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeBuilder<TPoco> {
   /// <summary>
   ///   Adds a specific rule for a member on the poco we're building rules for
   /// </summary>
   IEngineConfigurationTypeMemberBuilder<TPoco, TMember> Setup<TMember>(Expression<Func<TPoco, TMember>> expression);

   /// <summary>
   ///   Adds a rule for invoking an action on the poco on creation
   /// </summary>
   IEngineConfigurationTypeBuilder<TPoco> Invoke(Expression<Action<TPoco>> action);

   /// <summary>
   ///   Adds a rule for invoking a function on the poco on creation
   /// </summary>
   /// <param name="func"></param>
   IEngineConfigurationTypeBuilder<TPoco> Invoke<TReturn>(Expression<Func<TPoco, TReturn>> func);

   /// <summary>
   ///   Sets the data source from which instances of TPoco are created
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> ConstructWith<T>() where T : IDataSource<TPoco>;

   /// <summary>
   ///   Sets the data source from which instances of TPoco are created
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> ConstructWith<T>(params object[] args) where T : IDataSource<TPoco>;

   /*
   /// <summary>
   /// Allows manual configuration of a constructor's arguments
   /// </summary>
   /// <param name="creationExpr"></param>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Ctor(Expression<Func<TPoco>> creationExpr); */
}