using System.Numerics;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   Common implementation of <see cref="NumberSource{T}" /> and <see cref="NullableNumberSource{T}" />:
///   picks values between <see cref="Min" /> and <see cref="Max" /> for any built-in numeric type.
/// </summary>
/// <remarks>
///   Integer types (everything implementing <see cref="IBinaryInteger{TSelf}" />, up to 128 bit wide) are drawn
///   uniformly from the inclusive range, both bounds can be produced. Floating point types and <see cref="decimal" />
///   are drawn as <c>min + sample * (max - min)</c> with a sample in [0, 1), computed in a way that does not overflow
///   even for the whole range of the type.
/// </remarks>
/// <typeparam name="TResult">the produced type, either <typeparamref name="TNumber" /> or its nullable form</typeparam>
/// <typeparam name="TNumber">the numeric type</typeparam>
public abstract class NumberSourceBase<TResult, TNumber> : DataSourceBase<TResult>
   where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> {

   // IBinaryInteger<TSelf> is self constrained, so MakeGenericType would throw for non-integer types
   private static readonly bool _isBinaryInteger = typeof(TNumber)
      .GetInterfaces()
      .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBinaryInteger<>));

   protected NumberSourceBase(TNumber min, TNumber max)
      => SetMinMax(min, max);

   protected NumberSourceBase(TNumber min, TNumber max, int? nullCreationThreshold)
      : base(nullCreationThreshold)
      => SetMinMax(min, max);

   /// <summary>
   ///   Lower bound, inclusive.
   /// </summary>
   public TNumber Min { get; private set; }

   /// <summary>
   ///   Upper bound, inclusive for integer types.
   /// </summary>
   public TNumber Max { get; private set; }

   /// <summary>
   ///   Changes both bounds at once.
   /// </summary>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="max" /> is smaller than <paramref name="min" />, or one of them is not a finite number
   /// </exception>
   public NumberSourceBase<TResult, TNumber> SetMinMax(TNumber min, TNumber max) {
      if (!TNumber.IsFinite(min))
         throw new ArgumentOutOfRangeException(nameof(min), min, "The minimum must be a finite number.");

      if (!TNumber.IsFinite(max))
         throw new ArgumentOutOfRangeException(nameof(max), max, "The maximum must be a finite number.");

      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(max), max, $"The maximum must not be smaller than the minimum {min}.");

      Min = min;
      Max = max;
      return this;
   }

   /// <summary>
   ///   Changes the lower bound.
   /// </summary>
   public NumberSourceBase<TResult, TNumber> SetMin(TNumber min)
      => SetMinMax(min, Max);

   /// <summary>
   ///   Changes the upper bound.
   /// </summary>
   public NumberSourceBase<TResult, TNumber> SetMax(TNumber max)
      => SetMinMax(Min, max);

   protected override TResult GetNextValue(IGenerationContext? context)
      => (TResult) (object) (_isBinaryInteger ? Random.NextInclusive(Min, Max) : NextContinuous());

   /// <summary>
   ///   Interpolates between the bounds. Each product stays within the magnitude of its bound,
   ///   so the whole range of the type is safe from overflow.
   /// </summary>
   private TNumber NextContinuous() {
      var sample = TNumber.CreateChecked(Random.NextDouble());
      var value = (Min * (TNumber.One - sample)) + (Max * sample);
      return TNumber.Clamp(value, Min, Max);
   }
}

/// <summary>
///   Creates numbers of any built-in numeric type between a minimum and a maximum:
///   <c>NumberSource&lt;decimal&gt;</c>, <c>NumberSource&lt;byte&gt;</c>, <c>NumberSource&lt;UInt128&gt;</c>, <c>NumberSource&lt;Half&gt;</c>, ...
/// </summary>
/// <typeparam name="T">the numeric type</typeparam>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive for integer types</param>
public class NumberSource<T>(T min, T max) : NumberSourceBase<T, T>(min, max)
   where T : struct, INumber<T>, IMinMaxValue<T> {

   /// <summary>
   ///   Creates a source over the whole range of <typeparamref name="T" />.
   /// </summary>
   public NumberSource()
      : this(T.MinValue, T.MaxValue) { }
}

/// <summary>
///   Nullable counterpart of <see cref="NumberSource{T}" />, returns null now and then.
/// </summary>
/// <typeparam name="T">the numeric type</typeparam>
/// <param name="min">Minimum value, inclusive</param>
/// <param name="max">Maximum value, inclusive for integer types</param>
/// <param name="nullCreationThreshold">
///   Probability of null in percent, or null for the default, see <see cref="AutoPocoDefaults.NullCreationThreshold" />
/// </param>
public class NullableNumberSource<T>(T min, T max, int? nullCreationThreshold) : NumberSourceBase<T?, T>(min, max, nullCreationThreshold)
   where T : struct, INumber<T>, IMinMaxValue<T> {

   /// <summary>
   ///   Creates a source over the whole range of <typeparamref name="T" /> with the default null creation threshold.
   /// </summary>
   public NullableNumberSource()
      : this(T.MinValue, T.MaxValue, null) { }

   /// <summary>
   ///   Creates a source over the given range with the default null creation threshold.
   /// </summary>
   public NullableNumberSource(T min, T max)
      : this(min, max, null) { }

   /// <summary>
   ///   Creates a source over the whole range of <typeparamref name="T" /> with the given null creation threshold.
   /// </summary>
   public NullableNumberSource(int nullCreationThreshold)
      : this(T.MinValue, T.MaxValue, nullCreationThreshold) { }
}
