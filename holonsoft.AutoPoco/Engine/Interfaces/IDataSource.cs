namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IDataSource {
   object? InternalNext(IGenerationContext? context);
}

public interface IDataSource<T> : IDataSource { }