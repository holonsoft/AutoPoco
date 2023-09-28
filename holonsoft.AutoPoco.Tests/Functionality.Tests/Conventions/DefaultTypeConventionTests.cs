using FluentAssertions;
using Moq;
using System.Reflection;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class DefaultTypeConventionTests {
   private readonly DefaultTypeConvention _convention;
   private readonly Mock<ITypeConventionContext> _typeConventionContext;

   public DefaultTypeConventionTests() {
      _convention = new DefaultTypeConvention();
      _typeConventionContext = new Mock<ITypeConventionContext>();
   }

   [Fact]
   public void ApplyIgnoresPropertiesWithoutPublicSetter() {
      _typeConventionContext.SetupGet(x => x.Target).Returns(typeof(ClassWithPrivateSetters));
      _convention.Apply(_typeConventionContext.Object);
      _typeConventionContext.Verify(x => x.RegisterProperty(It.IsAny<PropertyInfo>()), Times.Never());
   }

   [Fact]
   public void ApplyIgnoresBaseProperties() {
      var count = 0;
      _typeConventionContext.SetupGet(x => x.Target).Returns(typeof(Class));
      _typeConventionContext.Setup(x => x.RegisterProperty(It.IsAny<PropertyInfo>()))
        .Callback(() => { count++; });

      _convention.Apply(_typeConventionContext.Object);

      count.Should().Be(1);
   }

   [Fact]
   public void ApplyIgnoresBaseFields() {
      var count = 0;
      _typeConventionContext.SetupGet(x => x.Target).Returns(typeof(Class));
      _typeConventionContext.Setup(x => x.RegisterField(It.IsAny<FieldInfo>()))
        .Callback(() => { count++; });

      _convention.Apply(_typeConventionContext.Object);

      count.Should().Be(1);
   }

   [Fact]
   public void ApplyRegistersInterfacePropertiesWhenRunningAgainstThatInterface() {
      _typeConventionContext.SetupGet(x => x.Target).Returns(typeof(ITestInterface));
      _convention.Apply(_typeConventionContext.Object);
      _typeConventionContext.Verify(x => x.RegisterProperty(It.IsAny<PropertyInfo>()), Times.Once());
   }

   public class BaseClass : IBaseTestInteface {
      public string? BaseField;

      public string? BaseProperty { get; set; }

      public string? BaseInterfaceProperty { get; set; }
   }

   public class ClassWithPrivateSetters {
      public string? Protected { get; protected set; }
      public string? Private { get; private set; }
      public string? Internal { get; private set; }
   }

   public class Class : BaseClass, ITestInterface {
      public string? TopField;

      public string? TopProperty { get; set; }

      public string? InterfaceProperty { get; set; }
   }

   public interface IBaseTestInteface {
      string? BaseInterfaceProperty { get; set; }
   }

   public interface ITestInterface : IBaseTestInteface {
      string? InterfaceProperty { get; set; }
   }
}