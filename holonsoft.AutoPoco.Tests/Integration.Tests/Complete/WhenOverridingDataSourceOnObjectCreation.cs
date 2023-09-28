using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenOverridingDataSourceOnObjectCreation {

   public WhenOverridingDataSourceOnObjectCreation() {
      _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.AddFromAssemblyContainingType<SimpleUser>();
      })
        .CreateSession();

      _results = _session.List<SimpleUser>(5)
        .Source(x => x.EmailAddress, new DummySource())
        .Get();
   }

   private readonly IGenerationSession _session;
   private readonly IList<SimpleUser> _results;

   [Fact]
   public void OverriddenDataSourceIsUsedForAllUsers() {
      for (var x = 0; x < 5; x++) {
         var user = _results[x];
         user.EmailAddress.Should().Be("Test" + x);
      }
   }

   private class DummySource : IDataSource<string> {
      private int _count;

      public object Next(IGenerationContext? context) => "Test" + _count++;
   }
}