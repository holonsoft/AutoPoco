using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Base;

/// <summary>
///   A data source that always returns the same, fixed value.
/// </summary>
/// <typeparam name="T">The member type.</typeparam>
public class ValueSource<T> : IDataSource<T> {
   private readonly T _value;

   /// <summary>
   ///   Initializes a new instance of the <see cref="ValueSource{T}" /> class.
   /// </summary>
   /// <param name="value">The value to return. Must not be null; use a lambda source for members that may be null.</param>
   public ValueSource(T value) {
      ArgumentNullException.ThrowIfNull(value);
      _value = value;
   }

   public IRandomNullEvaluator RandomNullEvaluator => throw new NotImplementedException();

   public object InternalNext(IGenerationContext? context) => _value!;
}
