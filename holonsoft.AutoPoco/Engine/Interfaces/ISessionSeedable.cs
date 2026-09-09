namespace holonsoft.AutoPoco.Engine.Interfaces;

/// <summary>
///   A data source whose random stream the engine seeds from the session seed. Implemented by
///   <see cref="DataSourceBase{T}" />: the engine seed is applied once and only when no seed was
///   set explicitly with <c>SetSeedToRandomValue</c>.
/// </summary>
public interface ISessionSeedable {
   void ApplySessionSeed(int seed);
}
