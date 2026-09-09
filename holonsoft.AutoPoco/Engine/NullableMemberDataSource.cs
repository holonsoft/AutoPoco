using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

/// <summary>
///   Decorates the data source of a member that allows null: with the probability of the evaluator
///   the member gets null, otherwise the inner source delivers the value.
///   Created by the engine when <see cref="NullableAnnotationSettings.RespectNullableAnnotations" /> is on.
/// </summary>
public sealed class NullableMemberDataSource : IDataSource {
   public NullableMemberDataSource(IDataSource inner, IRandomNullEvaluator randomNullEvaluator) {
      ArgumentNullException.ThrowIfNull(inner);
      ArgumentNullException.ThrowIfNull(randomNullEvaluator);

      Inner = inner;
      RandomNullEvaluator = randomNullEvaluator;
   }

   public IDataSource Inner { get; }

   public IRandomNullEvaluator RandomNullEvaluator { get; }

   public object? InternalNext(IGenerationContext? context)
      => RandomNullEvaluator.ShouldNextValueReturnNull()
         ? null
         : Inner.InternalNext(context);

   /// <summary>
   ///   Wraps the source when the settings are enabled and the member allows null. Returns the source
   ///   unchanged otherwise. Method members are never wrapped.
   /// </summary>
   /// <param name="source">the source producing the values</param>
   /// <param name="member">the member the source feeds</param>
   /// <param name="settings">the nullable annotation settings of the session</param>
   /// <param name="seed">the session seed, every member gets its own null pattern derived from it</param>
   public static IDataSource ForMember(IDataSource source, EngineTypeMember member, NullableAnnotationSettings? settings, int seed = AutoPocoDefaults.Seed) {
      ArgumentNullException.ThrowIfNull(source);
      ArgumentNullException.ThrowIfNull(member);

      if (settings is not { RespectNullableAnnotations: true })
         return source;

      var (allowsNull, declaringType) = member switch {
         EngineTypePropertyMember p => (NullabilityHelper.AllowsNull(p.PropertyInfo), p.PropertyInfo.DeclaringType),
         EngineTypeFieldMember f => (NullabilityHelper.AllowsNull(f.FieldInfo), f.FieldInfo.DeclaringType),
         _ => (false, null)
      };

      if (!allowsNull)
         return source;

      var evaluator = new DefaultRandomNullEvaluator(settings.NullCreationThreshold);
      // Every member gets its own deterministic seed, otherwise all nullable members of an object
      // would become null at the same time.
      evaluator.SetSeedToRandomValue(SeedDerivation.ForNullEvaluator(seed, declaringType, member.Name));

      return new NullableMemberDataSource(source, evaluator);
   }
}
