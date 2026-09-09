using holonsoft.AutoPoco.Configuration;

namespace holonsoft.AutoPoco.Engine.Interfaces;

/// <summary>
///   A registered member together with the data source that produces its values.
/// </summary>
public sealed record MemberSource(EngineTypeMember Member, IDataSource Source);

/// <summary>
///   A type factory that can take the values of registered members itself, e.g. to pass them to a constructor.
///   The <see cref="ObjectBuilder" /> offers the member sources once and does not set the consumed members again.
/// </summary>
public interface IMemberBoundFactory {
   /// <summary>
   ///   Offers the registered members with their data sources.
   /// </summary>
   /// <returns>the members whose values the factory takes care of</returns>
   IReadOnlyCollection<EngineTypeMember> BindMembers(IReadOnlyList<MemberSource> members);
}
