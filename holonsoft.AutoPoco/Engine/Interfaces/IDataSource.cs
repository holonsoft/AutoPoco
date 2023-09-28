namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IDataSource {
   object? Next(IGenerationContext? context);
}

public interface IDataSource<T> : IDataSource {
}