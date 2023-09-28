using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class CollectionContextTests {
   [Fact]
   public void ImposeImposesMemberOnAllItems() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionContext<SimpleUser, List<SimpleUser>>(
          mocks.Select(x => x.Object));

      // Call
      Expression<Func<SimpleUser, string>> expr = x => x.LastName;
      context.Impose(expr, "Test");

      // Verify they were all called
      foreach (var mock in mocks)
         mock.Verify(
           x => x.Impose(It.Is<Expression<Func<SimpleUser, string>>>(y => y == expr), It.Is<string>(y => y == "Test")),
           Times.Once());
   }

   [Fact]
   public void ImposeReturnsSelf() {
      // Set up
      var context =
        new CollectionContext<SimpleUser, List<SimpleUser>>(
          new List<IObjectGenerator<SimpleUser>>());

      // Call
      Expression<Func<SimpleUser, string>> expr = x => x.LastName;
      var returnContext = context.Impose(expr, "Test");

      // Verify
      returnContext.Should().Be(context);
   }

   [Fact]
   public void FirstReturnsSequence() {
      // Set up
      var context =
        new CollectionContext<SimpleUser, List<SimpleUser>>(
          new List<IObjectGenerator<SimpleUser>>());

      var sequence = context.First(10);
      sequence.Should().NotBeNull();
   }

   [Fact]
   public void RandomReturnsSequence() {
      // Set up
      var context =
        new CollectionContext<SimpleUser, List<SimpleUser>>(
          new List<IObjectGenerator<SimpleUser>>());

      var sequence = context.Random(10);
      sequence.Should().NotBeNull();
   }

   [Fact]
   public void GetListReturnsList() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionContext<SimpleUser, List<SimpleUser>>(
          mocks.Select(x => x.Object));

      var users = context.Get();

      users.Should().HaveCount(20);
   }

   [Fact]
   public void GetArrayReturnsArray() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionContext<SimpleUser, SimpleUser[]>(
          mocks.Select(x => x.Object));

      var users = context.Get();
      users.Length.Should().Be(20);
   }
}