using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public static class Use {
   public static TParamType? Source<TParamType, TSource>() where TSource : IDataSource<TParamType> => default;

#pragma warning disable IDE0060 // Remove unused parameter
   public static TParamType? Source<TParamType, TSource>(params object[] args) where TSource : IDataSource<TParamType> => default;
#pragma warning restore IDE0060 // Remove unused parameter
}