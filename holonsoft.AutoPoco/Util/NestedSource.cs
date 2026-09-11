using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Helpers for data sources that drive another data source internally, such as
///   <c>UrlSource</c> (the host) or <c>ExtendedEmailAddressSource</c> (first and last name).
/// </summary>
/// <remarks>
///   A nested source needs a stream of its own. Sharing the stream of the outer source, or of a
///   sibling, makes the parts of a generated value follow the same sequence of draws, which is the
///   very thing the per-member seeds of 6.0 got rid of.
/// </remarks>
internal static class NestedSource {
   /// <summary>
   ///   Derives the seed of a nested source from the seed of the outer source and what the nested one is used for.
   /// </summary>
   public static int Seed(int outerSeed, string purpose)
      => outerSeed ^ SeedDerivation.Fnv1a(purpose);

   /// <summary>
   ///   Puts a freshly created nested source on its own stream, derived from the default seed. Keeps a
   ///   source used outside of a session consistent with the same source inside one.
   /// </summary>
   public static TSource Seeded<TSource>(TSource source, string purpose)
      where TSource : ISessionSeedable {
      source.ApplySessionSeed(Seed(AutoPocoDefaults.Seed, purpose));
      return source;
   }
}
