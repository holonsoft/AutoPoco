using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   A data source backed by a lambda. Use it when a value is easier to express inline than as a dedicated
///   <see cref="IDataSource{T}" /> class. The lambda is invoked once per generated value and is fully in charge
///   of the result, including null. No random null injection happens here.
/// </summary>
/// <typeparam name="T">The member type the source produces.</typeparam>
public sealed class FuncSource<T> : IDataSource<T> {
   private readonly Func<IGenerationContext?, T> _factory;

   /// <summary>
   ///   Creates a source that ignores the generation context.
   /// </summary>
   public FuncSource(Func<T> factory) {
      ArgumentNullException.ThrowIfNull(factory);
      _factory = _ => factory();
   }

   /// <summary>
   ///   Creates a source that receives the current generation context, e.g. to build related objects
   ///   via <c>context.Single&lt;TOther&gt;()</c>.
   /// </summary>
   public FuncSource(Func<IGenerationContext?, T> factory) {
      ArgumentNullException.ThrowIfNull(factory);
      _factory = factory;
   }

   /// <summary>
   ///   Gets the next value from the lambda.
   /// </summary>
   public T Next(IGenerationContext? context) => _factory(context);

   object? IDataSource.InternalNext(IGenerationContext? context) => _factory(context);
}
