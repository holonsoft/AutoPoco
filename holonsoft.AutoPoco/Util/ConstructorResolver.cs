using System.Reflection;
using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Util;

/// <summary>
///   Matches registered members (properties, fields) to constructor parameters and picks the constructor
///   to create a type with. Used for records and other immutable types whose members can only be set
///   through a constructor.
/// </summary>
public static class ConstructorResolver {
   /// <summary>
   ///   All public instance constructors of a type.
   /// </summary>
   public static ConstructorInfo[] GetPublicConstructors(Type type) {
      ArgumentNullException.ThrowIfNull(type);
      return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
   }

   /// <summary>
   ///   A parameter matches a member when the names are equal ignoring case and the member value can be passed as the argument.
   /// </summary>
   public static bool Matches(ParameterInfo parameter, string memberName, Type memberType)
      => string.Equals(parameter.Name, memberName, StringComparison.OrdinalIgnoreCase)
         && parameter.ParameterType.IsAssignableFrom(memberType);

   public static bool Matches(ParameterInfo parameter, EngineTypeMember member)
      => !member.IsMethod && Matches(parameter, member.Name, GetMemberType(member));

   /// <summary>
   ///   True when any public constructor of the type has a parameter matching the given member.
   /// </summary>
   public static bool HasMatchingParameter(Type type, string memberName, Type memberType)
      => GetPublicConstructors(type)
         .Any(ctor => ctor.GetParameters().Any(p => Matches(p, memberName, memberType)));

   /// <summary>
   ///   The registered member a parameter is fed from, or null when none matches.
   /// </summary>
   public static EngineTypeMember? FindMember(ParameterInfo parameter, IEnumerable<EngineTypeMember> members)
      => members.FirstOrDefault(m => Matches(parameter, m));

   /// <summary>
   ///   Picks the constructor for a type given the registered members with a data source.
   ///   Preference, in this order: the constructor that consumes the most members which cannot be set otherwise
   ///   (no setter), then the one with the fewest parameters, then the one consuming the most members overall.
   ///   Without members this is the constructor with the fewest parameters, so a parameterless constructor
   ///   keeps winning for ordinary classes.
   /// </summary>
   /// <returns>the constructor, or null when the type has no public constructor</returns>
   public static ConstructorInfo? Resolve(Type type, IReadOnlyCollection<EngineTypeMember> members) {
      ArgumentNullException.ThrowIfNull(members);
      var relevant = members.Where(m => !m.IsMethod).ToList();

      return GetPublicConstructors(type)
         .Select(ctor => {
            var parameters = ctor.GetParameters();
            var matched = parameters.Select(p => FindMember(p, relevant)).OfType<EngineTypeMember>().ToList();
            return new {
               Constructor = ctor,
               ParameterCount = parameters.Length,
               Matches = matched.Count,
               RequiredMatches = matched.Count(m => !CanBeSet(m))
            };
         })
         .OrderByDescending(x => x.RequiredMatches)
         .ThenBy(x => x.ParameterCount)
         .ThenByDescending(x => x.Matches)
         .Select(x => x.Constructor)
         .FirstOrDefault();
   }

   /// <summary>
   ///   True when the member can be assigned after construction: fields always, properties with any setter (init and non-public included).
   /// </summary>
   public static bool CanBeSet(EngineTypeMember member)
      => member switch {
         EngineTypePropertyMember property => property.PropertyInfo.GetSetMethod(true) != null,
         EngineTypeFieldMember => true,
         _ => false
      };

   public static Type GetMemberType(EngineTypeMember member)
      => member switch {
         EngineTypePropertyMember property => property.PropertyInfo.PropertyType,
         EngineTypeFieldMember field => field.FieldInfo.FieldType,
         _ => throw new ArgumentException($"Member '{member.Name}' is neither a property nor a field.", nameof(member))
      };
}
