using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface IDataSourceFactory<TMember> {
   IDataSource<TMember> Create();
}