// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   Allows you to use another Source to generate an enumerable collection
/// </summary>
/// <typeparam name="TSource">
///   The type of the source.
/// </typeparam>
/// <typeparam name="T">
/// </typeparam>
public class EnumerableSource<TSource, T> : DataSourceBase<IEnumerable<T>>
  where TSource : IDataSource<T> {
   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The next in the generation of list of T
   /// </returns>
   public override IEnumerable<T> Next(IGenerationContext? context) {
      var count = Random.Next(_minCount, _maxCount + 1);
      for (var i = 0; i < count; i++)
         yield return (T) _source.Next(context)!;
   }

   /// <summary>
   ///   The args.
   /// </summary>
   private readonly object[] _args;

   /// <summary>
   ///   The max count.
   /// </summary>
   private readonly int _maxCount;

   /// <summary>
   ///   The min count.
   /// </summary>
   private readonly int _minCount;

   /// <summary>
   ///   The source.
   /// </summary>
   private readonly IDataSource<T> _source;

   /// <summary>
   ///   Initializes a new instance of the <see cref="EnumerableSource{TSource,T}" /> class.
   /// </summary>
   /// <param name="count">
   ///   The count.
   /// </param>
   public EnumerableSource(int count)
     : this(count, count, Array.Empty<object>()) {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="EnumerableSource{TSource,T}" /> class.
   /// </summary>
   /// <param name="count">
   ///   The count.
   /// </param>
   /// <param name="args">
   ///   The args.
   /// </param>
   public EnumerableSource(int count, object[] args)
     : this(count, count, args) {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="EnumerableSource{TSource,T}" /> class.
   /// </summary>
   /// <param name="min">
   ///   The min.
   /// </param>
   /// <param name="max">
   ///   The max.
   /// </param>
   public EnumerableSource(int min, int max)
     : this(min, max, Array.Empty<object>()) {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="EnumerableSource{TSource,T}" /> class.
   /// </summary>
   /// <param name="minCount">
   ///   The min count.
   /// </param>
   /// <param name="maxCount">
   ///   The max count.
   /// </param>
   /// <param name="args">
   ///   The args.
   /// </param>
   public EnumerableSource(int minCount, int maxCount, object[] args) {
      _minCount = minCount;
      _maxCount = maxCount;
      _args = args;

      var factory = new DataSourceFactory(typeof(TSource));
      factory.SetParams(_args);
      _source = (IDataSource<T>) factory.Build()! ?? throw new InvalidOperationException();
   }
}