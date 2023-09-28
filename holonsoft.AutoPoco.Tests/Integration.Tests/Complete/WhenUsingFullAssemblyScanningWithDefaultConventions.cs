using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingFullAssemblyScanningWithDefaultConventions {
   public WhenUsingFullAssemblyScanningWithDefaultConventions() =>
      // As default as it gets
      _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.AddFromAssemblyContainingType<SimpleUser>();
      })
        .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void SimpleUserCanBeCreated() {
      var user = _session.Single<SimpleUser>().Get();
      user.Should().NotBeNull();
   }

   [Fact]
   public void SimpleFieldClassCanBeCreated() {
      var obj = _session.Single<SimpleFieldClass>().Get();
      obj.Should().NotBeNull();
   }

   [Fact]
   public void SimplePropertyClassCanBeCreated() {
      var obj = _session.Single<SimplePropertyClass>().Get();
      obj.Should().NotBeNull();
   }

   [Fact]
   public void SimpleMethodClassInvokeCallsMethodWithParams() {
      var target = _session.Single<SimpleMethodClass>()
        .Invoke(x => x.SetSomething("Something"))
        .Get();

      target.Value.Should().Be("Something");
   }

   [Fact]
   public void SimpleMethodClassInvokeCallsMethodWithoutParams() {
      var target = _session.Single<SimpleMethodClass>()
        .Invoke(x => x.ReturnSomething())
        .Get();

      target.ReturnSomethingCalled.Should().BeTrue();
   }

   [Fact]
   public void SimpleMethodClassInvokeOnFirstCollectionCallsAction() {
      var items = _session.List<SimpleMethodClass>(100)
        .First(50)
        .Invoke(x => x.SetSomething("Something"))
        .Next(50)
        .Invoke(x => x.SetSomething("SomethingElse"))
        .All()
        .Get();

      Assert.True(items.Where(x => x.Value == "Something").Count() == 50 &&
                  items.Where(x => x.Value == "SomethingElse").Count() == 50);
   }

   [Fact]
   public void SimpleMethodClassInvokeOnRandomCollectionCallsAction() {
      var items = _session.List<SimpleMethodClass>(100)
        .Random(50)
        .Invoke(x => x.SetSomething("Something"))
        .Next(50)
        .Invoke(x => x.SetSomething("SomethingElse"))
        .All()
        .Get();

      Assert.True(items.Where(x => x.Value == "Something").Count() == 50 &&
                  items.Where(x => x.Value == "SomethingElse").Count() == 50);
   }

   [Fact]
   public void SimpleMethodClassInvokeOnEntireCollectionCallsAction() {
      var items = _session.List<SimpleMethodClass>(100)
        .Invoke(x => x.SetSomething("Something"))
        .Get();

      items.Where(x => x.Value == "Something").Count().Should().Be(100);
   }

   [Fact]
   public void SimpleMethodClassInvokeOnFirstCollectionCallsFunc() {
      var items = _session.List<SimpleMethodClass>(100)
        .First(50)
        .Invoke(x => x.ReturnSomething())
        .All()
        .Get();

      items.Where(x => x.ReturnSomethingCalled).Count().Should().Be(50);
   }

   [Fact]
   public void SimpleMethodClassInvokeOnRandomCollectionCallsFunc() {
      var items = _session.List<SimpleMethodClass>(100)
        .Random(50)
        .Invoke(x => x.ReturnSomething())
        .All()
        .Get();

      items.Where(x => x.ReturnSomethingCalled).Count().Should().Be(50);
   }

   [Fact]
   public void SimpleMethodClassInvokeOnEntireCollectionCallsFunc() {
      var items = _session.List<SimpleMethodClass>(100)
        .Invoke(x => x.ReturnSomething())
        .Get();

      items.Where(x => x.ReturnSomethingCalled).Count().Should().Be(100);
   }
}