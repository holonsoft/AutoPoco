// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomNumberGenerator.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util.Interfaces;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   The random number generator.
/// </summary>
public class RandomNumberGenerator : DataSourceBase<int>, IRandomNumberGenerator {
   /// <summary>
   ///   The current.
   /// </summary>
   private static IRandomNumberGenerator? _current;

   /// <summary>
   ///   Gets the current.
   /// </summary>
   public static IRandomNumberGenerator Current => _current ??= new RandomNumberGenerator();

   /// <summary>
   ///   Gets the next random number.
   /// </summary>
   /// <returns>
   ///   The <see cref="int" />.
   /// </returns>
   public int Next() => Random.Next();

   /// <summary>
   ///   Gets the next random number between zero and the given max value.
   /// </summary>
   /// <param name="maxValue">
   ///   The max value.
   /// </param>
   /// <returns>
   ///   The <see cref="int" />.
   /// </returns>
   public int Next(int maxValue) => Random.Next(maxValue);

   /// <summary>
   ///   Gets the next random number between the min value and the given max value.
   /// </summary>
   /// <param name="minValue">
   ///   The min value.
   /// </param>
   /// <param name="maxValue">
   ///   The max value.
   /// </param>
   /// <returns>
   ///   The <see cref="int" />.
   /// </returns>
   public int Next(int minValue, int maxValue)
      => Random.Next(minValue, maxValue);

   /// <summary>
   ///   The next double.
   /// </summary>
   /// <returns>
   ///   The <see cref="double" />.
   /// </returns>
   public double NextDouble()
      => Random.NextDouble();

   /// <summary>
   ///   Fills the buffer with a collection of random bytes.
   /// </summary>
   /// <param name="buffer">The buffer.</param>
   public void NextBytes(byte[] buffer)
      => Random.NextBytes(buffer);

   protected override int GetNextValue(IGenerationContext? context)
      => Random.Next();
}