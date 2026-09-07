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
   public static IDataSource ForMember(IDataSource source, EngineTypeMember member, NullableAnnotationSettings? settings) {
      ArgumentNullException.ThrowIfNull(source);
      ArgumentNullException.ThrowIfNull(member);

      if (settings is not { RespectNullableAnnotations: true })
         return source;

      var (allowsNull, memberKey) = member switch {
         EngineTypePropertyMember p => (NullabilityHelper.AllowsNull(p.PropertyInfo), MemberKey(p.PropertyInfo.DeclaringType, p.Name)),
         EngineTypeFieldMember f => (NullabilityHelper.AllowsNull(f.FieldInfo), MemberKey(f.FieldInfo.DeclaringType, f.Name)),
         _ => (false, string.Empty)
      };

      if (!allowsNull)
         return source;

      var evaluator = new DefaultRandomNullEvaluator(settings.NullCreationThreshold);
      // Every member gets its own deterministic seed, otherwise all nullable members of an object
      // would become null at the same time.
      evaluator.SetSeedToRandomValue(AutoPocoGlobalSettings.StandardSeed ^ StableHash(memberKey));

      return new NullableMemberDataSource(source, evaluator);
   }

   private static string MemberKey(Type? declaringType, string name)
      => $"{declaringType?.FullName}.{name}";

   /// <summary>
   ///   FNV-1a. string.GetHashCode is randomized per process and would break repeatable test data.
   /// </summary>
   private static int StableHash(string value) {
      unchecked {
         var hash = 2166136261u;
         foreach (var c in value) {
            hash ^= c;
            hash *= 16777619u;
         }

         return (int) hash;
      }
   }
}
