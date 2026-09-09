using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class DefaultTypeCtorConventionTests {
   private static Mock<ITypeConventionContext> Apply(Type target) {
      var context = new Mock<ITypeConventionContext>();
      context.SetupGet(x => x.Target).Returns(target);
      new DefaultComplexTypeCtorConvention().Apply(context.Object);
      return context;
   }

   [Fact]
   public void TypesWithConstructorsGetAResolvingCtorSource() {
      Apply(typeof(SampleCtorType)).Verify(x => x.SetFactory(typeof(CtorSource<SampleCtorType>)), Times.Once());
      Apply(typeof(SampleDefaultCtorType)).Verify(x => x.SetFactory(typeof(CtorSource<SampleDefaultCtorType>)), Times.Once());
      Apply(typeof(ImmutableUserRecord)).Verify(x => x.SetFactory(typeof(CtorSource<ImmutableUserRecord>)), Times.Once());
   }

   [Fact]
   public void TheConstructorIsNotPinnedByTheConvention() {
      Apply(typeof(SampleCtorType)).Verify(x => x.SetFactory(It.IsAny<Type>(), It.IsAny<object[]>()), Times.Never());
   }

   [Theory]
   [InlineData(typeof(int))]
   [InlineData(typeof(decimal))]
   [InlineData(typeof(string))]
   [InlineData(typeof(ISimpleInterface))]
   [InlineData(typeof(Stream))]
   public void PrimitivesStringsAndTypesWithoutPublicConstructorGetNoFactory(Type target) {
      Apply(target).Verify(x => x.SetFactory(It.IsAny<Type>()), Times.Never());
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
