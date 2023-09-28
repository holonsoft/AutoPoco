using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class CtorSourceTests {
   [Fact]
   public void CtorSourceCallsCtorRequestsArgumentsFromSession() {
      var source = new CtorSource<TestCtorObject>(
        typeof(TestCtorObject).GetConstructor(new[] { typeof(TestDependency) })!
      );

      var context = new Mock<IGenerationContext>();
      context.Setup(x => x.Next<TestDependency>()).Returns(new TestDependency());

      var result = source.Next(context.Object);

      result.Dependency.Should().NotBeNull();
   }
}

public class TestCtorObject(TestDependency dependency) {
   public TestDependency Dependency { get; } = dependency;
}
