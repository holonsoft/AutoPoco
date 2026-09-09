using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

/// <summary>
///   Arguments of <c>Invoke(c =&gt; c.Method(...))</c> in the configuration: markers, constants, variables, lambdas.
/// </summary>
public class WhenInvokingMethodsFromConfiguration {
   private static IGenerationSession Configure(Action<IEngineConfigurationTypeBuilder<SimpleMethodClass>> setup)
      => AutoPocoContainer.Configure(x => setup(x.Include<SimpleMethodClass>())).CreateSession();

   [Fact]
   public void AConstantArgumentArrivesAsItsValue() {
      var session = Configure(c => c.Invoke(m => m.SetSomething("Literal")));

      session.Next<SimpleMethodClass>().Value.ShouldBe("Literal");
   }

   [Fact]
   public void ACapturedVariableArrivesAsItsValue() {
      var captured = "captured";
      var session = Configure(c => c.Invoke(m => m.SetSomething(captured)));

      session.Next<SimpleMethodClass>().Value.ShouldBe("captured");
   }

   [Fact]
   public void ANullConstantArrivesAsNull() {
      var session = Configure(c => c.Invoke(m => m.SetSomething(null!)));

      session.Next<SimpleMethodClass>().Value.ShouldBeNull();
   }

   [Fact]
   public void UseFromRunsTheLambdaOncePerObject() {
      var counter = 0;
      Func<string> tick = () => $"call {++counter}";
      var session = Configure(c => c.Invoke(m => m.SetSomething(Use.From(tick))));

      var values = session.Collection<SimpleMethodClass>(3).Select(m => m.Value).ToList();

      values.ShouldBe(["call 1", "call 2", "call 3"]);
   }

   [Fact]
   public void UseFromWithContextCanReachTheSession() {
      var session = Configure(c => c.Invoke(m => m.SetSomething(Use.From(ctx => ctx.Single<SimpleUserRole>().Impose(r => r.Name, "from context").Get().Name))));

      session.Next<SimpleMethodClass>().Value.ShouldBe("from context");
   }

   [Fact]
   public void UseFromAndUseSourceCanBeMixed() {
      var session = Configure(c => c.Invoke(m => m.SetSomething(Use.From(() => "fixed"), Use.Source<string, FirstNameSource>()!)));

      var item = session.Next<SimpleMethodClass>();

      item.Value.ShouldBe("fixed");
      item.OtherValue.ShouldNotBeNullOrEmpty();
   }

   [Fact]
   public void UseSourceWithConstructorArgumentsStillWorks() {
      var session = Configure(c => c.Invoke(m => m.SetSomething(Use.Source<string, RandomStringSource>(5, 10)!)));

      var value = session.Next<SimpleMethodClass>().Value;

      value.ShouldNotBeNull().Length.ShouldBeInRange(5, 10);
   }

   [Fact]
   public void AForeignMethodCallIsRejectedWithAClearMessage() {
      var exception = Should.Throw<ArgumentException>(() => Configure(c => c.Invoke(m => m.SetSomething(new SimpleMethodClass().ReturnSomething()))));

      exception.Message.ShouldContain("Use.Source");
      exception.Message.ShouldContain("Use.From");
   }

   [Fact]
   public void AnUnknownMethodNameIsRejectedWithAClearMessage() {
      var builder = new EngineConfigurationTypeBuilder<SimpleMethodClass>();

      var exception = Should.Throw<ArgumentException>(() => builder.SetupMethod("DoesNotExist"));

      exception.Message.ShouldContain("DoesNotExist");
   }
}
