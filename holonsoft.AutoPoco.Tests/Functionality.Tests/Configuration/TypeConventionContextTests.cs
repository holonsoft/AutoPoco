using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class TypeConventionContextTests {
   private readonly Mock<IEngineConfigurationType> _typeMock;
   private readonly TypeConventionContext _context;

   public TypeConventionContextTests() {
      _typeMock = new Mock<IEngineConfigurationType>();
      _context = new TypeConventionContext(_typeMock.Object);
   }

   [Fact]
   public void RegisterFieldTypeMemberRegistered() {
      var field = typeof(TestClass).GetField("Field")!;
      _context.RegisterField(field);
      _typeMock.Verify(x => x.RegisterMember(It.Is<EngineTypeMember>(y => y.Name == field.Name)), Times.Once());
   }

   [Fact]
   public void RegisterPropertyTypePropertyRegistered() {
      var property = typeof(TestClass).GetProperty("Property")!;
      _context.RegisterProperty(property);
      _typeMock.Verify(x => x.RegisterMember(It.Is<EngineTypeMember>(y => y.Name == property.Name)), Times.Once());
   }

   [Fact]
   public void SetFactoryFactoryIsSet() {
      var type = new EngineConfigurationType(typeof(SimpleUser));
      var context = new TypeConventionContext(type);
      context.SetFactory(typeof(SimpleUserFactory));
      var factory = type.GetFactory()!.Build();

      Assert.True(factory is SimpleUserFactory);
   }

   [Fact]
   public void RegisterMethodTypeMethodRegistered() {
      var info = typeof(TestClass).GetMethod("Method")!;
      var typeMemberMock = new Mock<IEngineConfigurationTypeMember>();

      var context = new MethodInvocationContext();
      context.AddArgumentValue(5);

      var count = 0;
      _typeMock.Setup(x => x.GetRegisteredMember(It.IsAny<EngineTypeMember>()))
        .Returns(() => {
           if (count == 0) {
              count++;
              return null!;
           }

           return typeMemberMock.Object;
        });
      _context.RegisterMethod(info, context);
      _typeMock.Verify(x => x.RegisterMember(It.Is<EngineTypeMember>(y => y.Name == info.Name)), Times.Once());
   }

   [Fact]
   public void TargetReturnsConfigurationType() {
      _typeMock.SetupGet(x => x.RegisteredType).Returns(typeof(TestClass));
      _context.Target.Should().Be(typeof(TestClass));
      //Assert.AreEqual(typeof(TestClass), _context.Target);
   }

   public class TestClass {
      public required string Field;

      public required string Property { get; set; }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
      public void Method(int value) { }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
   }
}

public class SimpleUserFactory : IDataSource<SimpleUser> {
   public object InternalNext(IGenerationContext? context) => throw new NotImplementedException();
}