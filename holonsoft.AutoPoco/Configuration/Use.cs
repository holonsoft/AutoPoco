using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

/// <summary>
///   Markers for the arguments of <c>Invoke(c =&gt; c.Method(...))</c> in the configuration. They are never
///   executed, the configuration reads the expression tree and replaces every marker with the data source
///   it stands for.
/// </summary>
public static class Use {
   /// <summary>
   ///   The argument comes from a data source created with its default constructor.
   /// </summary>
   public static TParamType? Source<TParamType, TSource>() where TSource : IDataSource<TParamType> => default;

#pragma warning disable IDE0060 // Remove unused parameter
   /// <summary>
   ///   The argument comes from a data source created with the given constructor arguments.
   /// </summary>
   public static TParamType? Source<TParamType, TSource>(params object[] args) where TSource : IDataSource<TParamType> => default;

   /// <summary>
   ///   The argument is computed by the lambda, once per invocation.
   /// </summary>
   public static TParamType From<TParamType>(Func<TParamType> factory) => default!;

   /// <summary>
   ///   The argument is computed by the lambda from the generation context, once per invocation.
   /// </summary>
   public static TParamType From<TParamType>(Func<IGenerationContext, TParamType> factory) => default!;
#pragma warning restore IDE0060 // Remove unused parameter
}
