using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenCreatingObjectsWithHaveAnInterfaceThatIsNotConfigured {
   public WhenCreatingObjectsWithHaveAnInterfaceThatIsNotConfigured()
      => _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => { c.UseDefaultConventions(); });
         x.Include<SimpleBaseClass>();
      })
        .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void DefaultInterfaceRulesCascadeOntoImplementingTypes() {
      var obj = _session.Single<SimpleBaseClass>().Get();
      obj.InterfaceValue.Should().NotBeNull();
   }
}