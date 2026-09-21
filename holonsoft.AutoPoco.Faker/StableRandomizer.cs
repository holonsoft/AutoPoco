using Bogus;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Faker;

/// <summary>
///   A Bogus randomizer that draws from AutoPoco's own <see cref="StableRandom" /> instead of
///   <see cref="System.Random" />.
/// </summary>
/// <remarks>
///   Two reasons. First, the sequence of a seed is then defined by AutoPoco and stays the same across .NET
///   versions, which is the promise the rest of the library makes. Second, every instance owns its stream:
///   the static <c>Randomizer.Seed</c> of Bogus is never read and never written, so two sources, two sessions
///   or two threads cannot pull each other's values.
///   What this does not buy: the values themselves also depend on the locale data inside the Bogus package,
///   so the same seed gives the same values only as long as the Bogus version stays the same. That is why
///   the package reference is pinned to an exact version.
/// </remarks>
internal sealed class StableRandomizer : Randomizer {
   /// <summary>
   ///   Creates a randomizer on its own stream. The base constructor builds a <see cref="System.Random" />
   ///   that is replaced right away, so nothing is ever drawn from it.
   /// </summary>
   public StableRandomizer(int seed)
      : base(seed)
      => localSeed = new StableRandom(seed);
}
