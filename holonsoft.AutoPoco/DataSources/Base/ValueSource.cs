// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSource.cs" company="">
//   
// </copyright>
// <summary>
//   The value source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   The value source.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
/// <remarks>
///   Initializes a new instance of the <see cref="ValueSource{T}" /> class.
/// </remarks>
/// <param name="value">
///   The value.
/// </param>
#pragma warning disable CA2208

public class ValueSource<T>(T value) : IDataSource<T> {
   public IRandomNullEvaluator RandomNullEvaluator => throw new NotImplementedException();

   public object InternalNext(IGenerationContext? context)
      => value != null
            ? value
            : throw new ArgumentNullException();
}