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

public class ObjectFieldSetFromSourceActionTests {
   private readonly Mock<IDataSource> _sourceMock;
   private readonly GenerationContext _context;
   private readonly IGenerationContextNode _parentNode;
   private readonly ObjectFieldSetFromSourceAction _action;

   public ObjectFieldSetFromSourceActionTests() {
      _sourceMock = new Mock<IDataSource>();
      _parentNode = new TypeGenerationContextNode(null, null);
      _context = new GenerationContext(null!, _parentNode);

      _action = new ObjectFieldSetFromSourceAction((EngineTypeFieldMember)
        ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!), _sourceMock.Object);
   }

   [Fact]
   public void EnactSetsFieldWithSourceValue() {
      _sourceMock.Setup(x => x.Next(It.IsAny<IGenerationContext>())).Returns("Test");

      var target = new SimpleFieldClass();
      _action.Enact(_context, target);
      target.SomeField.Should().Be("Test");
   }

   [Fact]
   public void EnactCreateTypeFieldContextIntoSession() {
      var target = new SimpleFieldClass();

      _action.Enact(_context, target);

      _sourceMock.Verify(x => x.Next(It.Is<IGenerationContext>(y =>
          y.Node is TypeFieldGenerationContextNode &&
          y.Node.Parent == _parentNode)),
        Times.Once());
   }
}