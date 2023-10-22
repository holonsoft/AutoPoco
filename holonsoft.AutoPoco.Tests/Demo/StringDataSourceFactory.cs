using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Demo;

internal class StringDataSourceFactory : IDataSourceFactory<string> {
   private readonly IDataSource<string> _dataSource = new CapitalSource();

   public IDataSource<string> Create()
      => _dataSource;
}