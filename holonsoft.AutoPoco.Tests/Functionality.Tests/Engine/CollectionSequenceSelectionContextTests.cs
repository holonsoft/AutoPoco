using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class CollectionSequenceSelectionContextTests {

   public CollectionSequenceSelectionContextTests() => _parentContextMock = new Mock<ICollectionContext<SimpleUser, List<SimpleUser>>>();

   private readonly Mock<ICollectionContext<SimpleUser, List<SimpleUser>>> _parentContextMock;

   [Fact]
   public void ImposeImposesMemberOnAllCurrentItems() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionSequenceSelectionContext<SimpleUser, List<SimpleUser>>(
          _parentContextMock.Object,
          mocks.Select(x => x.Object),
          10);

      // Call
      Expression<Func<SimpleUser, string>> expr = x => x.LastName;
      context.Impose(expr, "Test");

      // Verify the first 10 in the sequence were called
      for (var x = 0; x < 10; x++) {
         var mock = mocks[x];
         mock.Verify(
           m => m.Impose(It.Is<Expression<Func<SimpleUser, string>>>(y => y == expr), It.Is<string>(y => y == "Test")),
           Times.Once());
      }

      // Verify the last 10 in the sequence were not called
      for (var x = 10; x < 20; x++) {
         var mock = mocks[x];
         mock.Verify(
           m => m.Impose(It.Is<Expression<Func<SimpleUser, string>>>(y => y == expr), It.Is<string>(y => y == "Test")),
           Times.Never());
      }
   }

   [Fact]
   public void AfterCreationRemainingIsExpectedValue() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionSequenceSelectionContext<SimpleUser, List<SimpleUser>>(
          _parentContextMock.Object,
          mocks.Select(x => x.Object),
          10);

      context.Remaining.Should().Be(10);
   }

   [Fact]
   public void AfterNextRemainingIsExpectedValue() {
      // Add 20 mocks to a list
      var mocks = new List<Mock<IObjectGenerator<SimpleUser>>>();
      for (var x = 0; x < 20; x++)
         mocks.Add(new Mock<IObjectGenerator<SimpleUser>>());

      // Set up
      var context =
        new CollectionSequenceSelectionContext<SimpleUser, List<SimpleUser>>(
          _parentContextMock.Object,
          mocks.Select(x => x.Object),
          10);

      // Forward ho
      context.Next(5);
      context.Remaining.Should().Be(5);
   }

   [Fact]
   public void AllReturnsParent() {
      // Set up
      var context =
        new CollectionSequenceSelectionContext<SimpleUser, List<SimpleUser>>(
          _parentContextMock.Object,
          new List<IObjectGenerator<SimpleUser>>(),
          0);

      var parent = context.All();

      parent.Should().Be(_parentContextMock.Object);
   }

   [Fact]
   public void ImposeReturnsSelf() {
      // Set up
      var context =
        new CollectionSequenceSelectionContext<SimpleUser, List<SimpleUser>>(
          _parentContextMock.Object,
          new List<IObjectGenerator<SimpleUser>>(),
          0);

      // Call
      Expression<Func<SimpleUser, string>> expr = x => x.LastName;
      var returnContext = context.Impose(expr, "Test");

      // Verify
      returnContext.Should().Be(context);
   }
}