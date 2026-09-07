using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class FuncSourceTests {
   [Fact]
   public void NextReturnsTheLambdaResult()
      => new FuncSource<string>(() => "value").Next(null).ShouldBe("value");

   [Fact]
   public void LambdaRunsOncePerValue() {
      var counter = 0;
      var source = new FuncSource<int>(() => ++counter);

      var values = Enumerable.Range(0, 5).Select(_ => source.Next(null)).ToList();

      values.ShouldBe([1, 2, 3, 4, 5]);
      counter.ShouldBe(5);
   }

   [Fact]
   public void ContextIsPassedToTheLambda() {
      var context = new Mock<IGenerationContext>().Object;
      IGenerationContext? seen = null;
      var source = new FuncSource<int>(ctx => { seen = ctx; return 1; });

      source.Next(context);

      seen.ShouldBeSameAs(context);
   }

   [Fact]
   public void ContextlessLambdaAlsoWorksWithAContext() {
      var context = new Mock<IGenerationContext>().Object;
      new FuncSource<int>(() => 42).Next(context).ShouldBe(42);
   }

   [Fact]
   public void NullFromTheLambdaIsPassedThroughUnchanged() {
      var source = new FuncSource<string?>(() => null);

      source.Next(null).ShouldBeNull();
      ((IDataSource) source).InternalNext(null).ShouldBeNull();
   }

   [Fact]
   public void NullableValueTypesAreNotRandomlyNulled() {
      var source = new FuncSource<int?>(() => 3);

      Enumerable.Range(0, 200).Select(_ => source.Next(null)).ShouldAllBe(x => x == 3);
   }

   [Fact]
   public void InternalNextReturnsTheSameAsNext() {
      IDataSource source = new FuncSource<double>(() => 1.5);
      source.InternalNext(null).ShouldBe(1.5);
   }

   [Fact]
   public void NullLambdaIsRejected() {
      Action withoutContext = () => new FuncSource<int>((Func<int>) null!);
      Action withContext = () => new FuncSource<int>((Func<IGenerationContext?, int>) null!);

      Should.Throw<ArgumentNullException>(withoutContext);
      Should.Throw<ArgumentNullException>(withContext);
   }

   [Fact]
   public void ExceptionsFromTheLambdaSurface() {
      var source = new FuncSource<int>(() => throw new InvalidOperationException("boom"));
      Action act = () => source.Next(null);
      Should.Throw<InvalidOperationException>(act).Message.ShouldBe("boom");
   }
}
