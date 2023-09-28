using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenRequestingClosedGenericType {

   public WhenRequestingClosedGenericType()
      => _session = AutoPocoContainer
            .Configure(x => { x.Include<OpenGeneric<object>>(); })
            .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void CreatedObjectIsReturned() {
      var created = _session.Single<OpenGeneric<object>>().Get();
      created.Should().NotBeNull();
   }
}