namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IDataSource {
   object? InternalNext(IGenerationContext? context);
}

/// <summary>
///   Typed marker for a data source. Covariant, so a source of <c>string</c> also serves a <c>string?</c> member.
/// </summary>
public interface IDataSource<out T> : IDataSource { }