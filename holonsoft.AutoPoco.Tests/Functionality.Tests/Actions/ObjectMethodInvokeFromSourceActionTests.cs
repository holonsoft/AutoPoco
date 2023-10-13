using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Actions;

public class ObjectMethodInvokeFromSourceActionTests {
   private readonly Mock<IDataSource> _sourceMock;
   private readonly GenerationContext _context;
   private readonly IGenerationContextNode _parentNode;
   private readonly EngineTypeMethodMember _doubleArgMethod;
   private readonly ObjectMethodInvokeFromSourceAction _doubleArgAction;

   public ObjectMethodInvokeFromSourceActionTests() {
      _sourceMock = new Mock<IDataSource>();
      _parentNode = new TypeGenerationContextNode(null, null);
      _context = new GenerationContext(null!, _parentNode);

      _doubleArgMethod = (EngineTypeMethodMember) ReflectionHelper.GetMember(
        typeof(SimpleMethodClass).GetMethod("SetSomething", new[] { typeof(string), typeof(string) })!);

      _doubleArgAction =
        new ObjectMethodInvokeFromSourceAction(_doubleArgMethod, new[] { _sourceMock.Object, _sourceMock.Object });
   }

   [Fact]
   public void SharedDataSourceWithTwoParamsNextInvokedWithWrappedUpSessionTwice() {
      var target = new SimpleMethodClass();
      _doubleArgAction.Enact(_context, target);

      _sourceMock.Verify(
        x => x.InternalNext(It.Is<IGenerationContext>(y =>
          y.Node is TypeMethodGenerationContextNode && y.Node.Parent == _parentNode)), Times.Exactly(2));
   }

   [Fact]
   public void SharedDataSourceWithTwoParamsFirstParamPassedCorrectly() {
      var target = new SimpleMethodClass();
      var callCount = 0;
      _sourceMock.Setup(x => x.InternalNext(It.IsAny<IGenerationContext>())).Returns(() => {
         callCount++;
         return callCount.ToString();
      });

      _doubleArgAction.Enact(_context, target);

      target.Value.Should().Be("1");
   }

   [Fact]
   public void SharedDataSourceWithTwoParamsSecondParamPassedCorrectly() {
      var target = new SimpleMethodClass();
      var callCount = 0;
      _sourceMock.Setup(x => x.InternalNext(It.IsAny<IGenerationContext>())).Returns(() => {
         callCount++;
         return callCount.ToString();
      });

      _doubleArgAction.Enact(_context, target);

      target.OtherValue.Should().Be("2");
   }
}
