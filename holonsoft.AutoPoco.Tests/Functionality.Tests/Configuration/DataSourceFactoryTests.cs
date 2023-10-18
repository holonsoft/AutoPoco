using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class DataSourceFactoryTests {
   [Fact]
   public void BuildReturnsNewFactory() {
      var factory = new AutoPocoDataSourceFactory(typeof(BlankDataSource));
      var source = factory.Build() as BlankDataSource;

      source.Should().NotBeNull();
   }
}