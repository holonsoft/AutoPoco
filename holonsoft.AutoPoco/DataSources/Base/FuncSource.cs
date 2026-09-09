using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   A data source backed by a lambda. The lambda is in charge of the value, including null.
/// </summary>
public sealed class FuncSource<T> : IDataSource<T> {
   private readonly Func<IGenerationContext?, T> _factory;
   private readonly bool _needsContext;

   /// <summary>
   ///   Creates a source that ignores the generation context.
   /// </summary>
   public FuncSource(Func<T> factory) {
      ArgumentNullException.ThrowIfNull(factory);
      _factory = _ => factory();
   }

   /// <summary>
   ///   Creates a source that receives the current generation context, e.g. to build related objects
   ///   via <c>context.Single&lt;TOther&gt;()</c>. Such a source only works inside a session.
   /// </summary>
   public FuncSource(Func<IGenerationContext, T> factory) {
      ArgumentNullException.ThrowIfNull(factory);
      _factory = context => factory(context!);
      _needsContext = true;
   }

   /// <summary>
   ///   Gets the next value from the lambda.
   /// </summary>
   /// <exception cref="InvalidOperationException">the lambda takes the generation context and the source was used outside of a session</exception>
   public T Next(IGenerationContext? context) {
      if (_needsContext && context is null)
         throw new InvalidOperationException(
            $"The lambda behind this FuncSource<{typeof(T).Name}> takes the generation context, so the source only works inside a session, not standalone.");

      return _factory(context);
   }

   object? IDataSource.InternalNext(IGenerationContext? context) => Next(context);
}
