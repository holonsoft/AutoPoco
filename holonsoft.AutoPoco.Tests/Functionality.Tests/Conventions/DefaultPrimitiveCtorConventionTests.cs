using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class DefaultPrimitiveCtorConventionTests {
   private readonly DefaultPrimitiveCtorConvention _convention = new();

   private Mock<ITypeConventionContext> Apply(Type target) {
      var context = new Mock<ITypeConventionContext>();
      context.SetupGet(x => x.Target).Returns(target);
      _convention.Apply(context.Object);
      return context;
   }

   [Theory]
   [InlineData(typeof(int), typeof(DefaultSource<int>))]
   [InlineData(typeof(long), typeof(DefaultSource<long>))]
   [InlineData(typeof(bool), typeof(DefaultSource<bool>))]
   [InlineData(typeof(char), typeof(DefaultSource<char>))]
   [InlineData(typeof(double), typeof(DefaultSource<double>))]
   [InlineData(typeof(byte), typeof(DefaultSource<byte>))]
   public void ApplyUsesDefaultSourceForPrimitives(Type target, Type expectedFactory)
      => Apply(target).Verify(x => x.SetFactory(expectedFactory), Times.Once());

   [Fact]
   public void ApplyTreatsDecimalLikeAPrimitive()
      => Apply(typeof(decimal)).Verify(x => x.SetFactory(typeof(DefaultSource<decimal>)), Times.Once());

   [Fact]
   public void ApplyUsesDefaultStringSourceForString()
      => Apply(typeof(string)).Verify(x => x.SetFactory(typeof(DefaultStringSource)), Times.Once());

   [Theory]
   [InlineData(typeof(object))]
   [InlineData(typeof(DateTime))]
   [InlineData(typeof(Guid))]
   [InlineData(typeof(int?))]
   [InlineData(typeof(List<int>))]
   public void ApplyLeavesNonPrimitivesAlone(Type target) {
      var context = Apply(target);
      context.Verify(x => x.SetFactory(It.IsAny<Type>()), Times.Never());
      context.Verify(x => x.SetFactory(It.IsAny<Type>(), It.IsAny<object[]>()), Times.Never());
   }
}
