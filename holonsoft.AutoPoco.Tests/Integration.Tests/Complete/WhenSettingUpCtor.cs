using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenSettingUpCtor {
   private IGenerationSession? _session;

   [Fact]
   public void WithSourceSourceIsUsedToCreateObject() {
      _session = AutoPocoContainer.Configure(x => x.Include<SimpleCtorClass>()
        .ConstructWith<TestFactory>()).CreateSession();

      var result = _session.Next<SimpleCtorClass>();
      result.ReadOnlyProperty.Should().Be("one");
      result.SecondaryProperty.Should().Be("two");
   }

   [Fact]
   public void WithNoConfigLeastGreedyConstructorWithDefaultConventionsIsUsedByDefault() {
      _session = AutoPocoContainer.CreateDefaultSession();

      var result = _session.Next<SimpleCtorClass>();

      Assert.NotNull(result.ReadOnlyProperty);
      Assert.Null(result.SecondaryProperty);
   }
}

public class TestFactory : IDataSource<SimpleCtorClass> {
   public object InternalNext(IGenerationContext? context) => new SimpleCtorClass("one", "two");
}