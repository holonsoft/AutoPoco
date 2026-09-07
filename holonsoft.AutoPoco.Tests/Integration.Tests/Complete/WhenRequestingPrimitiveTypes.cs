using Shouldly;
using Xunit;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenRequestingPrimitiveTypes {
   [Fact]
   public void RequestingStringUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<string>();
      Assert.NotNull(result);
   }

   [Fact]
   public void RequestingIntUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<int>();
      result.ShouldBe(0);
   }

   [Fact]
   public void RequestingDoubleUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<double>();
      result.ShouldBe(0);
   }

   [Fact]
   public void RequestingFloatUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<float>();
      result.ShouldBe(0);
   }

   [Fact]
   public void RequestingCharUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<char>();
      result.ShouldBe((char) 0);
   }

   [Fact]
   public void RequestingBoolUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<bool>();
      result.ShouldBe(false);
   }

   [Fact]
   public void RequestingShortUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<short>();
      result.ShouldBe((short) 0);
   }

   [Fact]
   public void RequestingDecimalUsesDefaultSource() {
      var session = AutoPocoContainer.Configure(x => { }).CreateSession();
      var result = session.Next<decimal>();
      result.ShouldBe(0);
   }
}