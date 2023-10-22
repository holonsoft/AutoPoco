using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IEngineConfigurationTypeMemberBuilder<TPoco, TMember> {
   /// <summary>
   ///   Uses the specified data source for member values
   /// </summary>
   /// <typeparam name="TSource"></typeparam>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Use<TSource>() where TSource : IDataSource<TMember>;

   /// <summary>
   ///   Uses the specified data source for member values (with args)
   /// </summary>
   /// <typeparam name="TSource"></typeparam>
   /// <param name="args"></param>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(params object[] args) where TSource : IDataSource<TMember>;

   /// <summary>
   /// Support for fluent lambdas
   /// </summary>
   /// <typeparam name="TSource"></typeparam>
   /// <param name="action"></param>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(Action<TSource> action) where TSource : IDataSource<TMember>;

   IEngineConfigurationTypeBuilder<TPoco> Use<TSource>(IDataSourceFactory<TMember> userDefinedFactory) where TSource : IDataSource<TMember>;

   /// <summary>
   ///   Allows this property to be set by convention
   /// </summary>
   /// <returns></returns>
   IEngineConfigurationTypeBuilder<TPoco> Default();
}