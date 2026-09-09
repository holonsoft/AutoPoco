namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Derives the seed of a single data source from the session seed and the place the source is used at.
///   Every member gets its own stream, so two members with the same source type do not produce the same
///   values, and the values of a member do not depend on which other members exist.
/// </summary>
public static class SeedDerivation {
   public static int ForMember(int seed, Type type, string memberName)
      => seed ^ Fnv1a($"{Name(type)}.{memberName}");

   public static int ForNullEvaluator(int seed, Type? type, string memberName)
      => seed ^ Fnv1a($"{Name(type)}.{memberName}#null");

   public static int ForMethodArgument(int seed, Type type, string methodName, int argumentIndex)
      => seed ^ Fnv1a($"{Name(type)}.{methodName}({argumentIndex})");

   public static int ForType(int seed, Type type)
      => seed ^ Fnv1a($"{Name(type)}#factory");

   /// <summary>
   ///   FNV-1a. string.GetHashCode is randomized per process and would break repeatable test data.
   /// </summary>
   public static int Fnv1a(string value) {
      ArgumentNullException.ThrowIfNull(value);
      unchecked {
         var hash = 2166136261u;
         foreach (var c in value) {
            hash ^= c;
            hash *= 16777619u;
         }

         return (int) hash;
      }
   }

   private static string Name(Type? type)
      => type?.FullName ?? type?.Name ?? string.Empty;
}
