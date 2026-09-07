using System.Linq.Expressions;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Extensions;

/// <summary>
///   Lambda overloads for the generation time API. They wrap the lambda in a <see cref="FuncSource{T}" />
///   so a value can be computed per generated object without writing a data source class.
/// </summary>
public static class GenerationExtensions {
   extension<TPoco>(IObjectGenerator<TPoco> generator) {
      /// <summary>
      ///   Overrides the data source of a member for this generation scope with a lambda.
      /// </summary>
      public IObjectGenerator<TPoco> Source<TMember>(Expression<Func<TPoco, TMember>> memberExpr, Func<TMember> factory)
         => generator.Source(memberExpr, new FuncSource<TMember>(factory));

      /// <summary>
      ///   Overrides the data source of a member for this generation scope with a lambda that receives the generation context.
      /// </summary>
      public IObjectGenerator<TPoco> Source<TMember>(Expression<Func<TPoco, TMember>> memberExpr, Func<IGenerationContext?, TMember> factory)
         => generator.Source(memberExpr, new FuncSource<TMember>(factory));
   }

   extension<TPoco, TCollection>(ICollectionContext<TPoco, TCollection> context) where TCollection : ICollection<TPoco> {
      /// <summary>
      ///   Overrides the data source of a member for every item of the collection with a lambda.
      ///   The lambda runs once per item, so a counter or a random value gives each item its own value.
      /// </summary>
      public ICollectionContext<TPoco, TCollection> Source<TMember>(Expression<Func<TPoco, TMember>> memberExpr, Func<TMember> factory)
         => context.Source(memberExpr, new FuncSource<TMember>(factory));

      /// <summary>
      ///   Overrides the data source of a member for every item of the collection with a lambda that receives the generation context.
      /// </summary>
      public ICollectionContext<TPoco, TCollection> Source<TMember>(Expression<Func<TPoco, TMember>> memberExpr, Func<IGenerationContext?, TMember> factory)
         => context.Source(memberExpr, new FuncSource<TMember>(factory));
   }
}
