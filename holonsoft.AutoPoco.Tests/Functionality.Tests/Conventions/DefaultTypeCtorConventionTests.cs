using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class DefaultTypeCtorConventionTests {
   [Fact]
   public void DefaultTypeCtorSeeksLeastGreedyCtor() {
      var convention = new DefaultComplexTypeCtorConvention();
      var context = new Mock<ITypeConventionContext>();
      context.SetupGet(x => x.Target).Returns(typeof(SampleCtorType));
      convention.Apply(context.Object);

      context.Verify(x => x.SetFactory(
        It.Is<Type>(type => type == typeof(CtorSource<SampleCtorType>)),
        typeof(SampleCtorType).GetConstructor(new[] { typeof(int) })!), Times.Once());
   }

   [Fact]
   public void DefaultTypeCtorUsesDefaultCtorIfAvailable() {
      var convention = new DefaultComplexTypeCtorConvention();
      var context = new Mock<ITypeConventionContext>();
      context.SetupGet(x => x.Target).Returns(typeof(SampleDefaultCtorType));
      convention.Apply(context.Object);

      context.Verify(x => x.SetFactory(
        It.Is<Type>(type => type == typeof(CtorSource<SampleDefaultCtorType>)),
        typeof(SampleDefaultCtorType).GetConstructor(Type.EmptyTypes)!), Times.Once());
   }
}

#pragma warning disable IDE0060 // Remove unused parameter

public class SampleDefaultCtorType {
   public SampleDefaultCtorType() {
   }

   public SampleDefaultCtorType(int x) {
   }

   public SampleDefaultCtorType(int x, int y) {
   }

   public SampleDefaultCtorType(int x, int y, int z) {
   }
}

public class SampleCtorType {
   public SampleCtorType(int x) {
   }

   public SampleCtorType(int x, int y) {
   }

   public SampleCtorType(int x, int y, int z) {
   }
}

#pragma warning restore IDE0060 // Remove unused parameter
