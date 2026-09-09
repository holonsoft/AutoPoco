namespace holonsoft.AutoPoco.Configuration;

/// <summary>
///   Library defaults. Read-only on purpose: a session seed is configured with <c>UseSeed</c> on the
///   configuration builder or <c>CreateSession(recursionLimit, seed)</c>, null thresholds per source or
///   with <c>RespectNullableAnnotations(threshold)</c>.
/// </summary>
public static class AutoPocoDefaults {
   /// <summary>
   ///   Seed of every factory and of every data source used standalone.
   /// </summary>
   public const int Seed = 1337;

   /// <summary>
   ///   Probability in percent that a nullable source returns null.
   /// </summary>
   public const int NullCreationThreshold = 15;

   /// <summary>
   ///   Recursion limit of a session created without one.
   /// </summary>
   public const int RecursionLimit = 5;
}
