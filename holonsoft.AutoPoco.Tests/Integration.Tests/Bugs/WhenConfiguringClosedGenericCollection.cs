using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Extensions;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Bugs;

public class WhenConfiguringClosedGenericCollection {
   [Fact]
   public void ObjectWithCollectionAsAPropertyCanBeRequestedFromSession() {
      var session = AutoPocoContainer.Configure(c =>
        c.Include<TestObject>()
          .Setup(x => x.Children).Collection(3, 3)).CreateSession();

      var result = session.Next<TestObject>();
      result.Should().NotBeNull();
      result.Children.Should().HaveCount(3);
   }

   public class TestObject {
      public required ClosedGenericCollection Children { get; set; }
   }

   public class ClosedGenericCollection : List<int> { }
}