// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomStringSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The random integer source. Just for compatibility reasons 
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="RandomNumberSource" /> class.
/// </remarks>
/// <param name="min">
///   The min.
/// </param>
/// <param name="max">
///   The max.
/// </param>
public class RandomNumberSource(int min, int max) : IntegerSource(min, max) {

   public RandomNumberSource()
      : this(int.MinValue, int.MaxValue) { }
}