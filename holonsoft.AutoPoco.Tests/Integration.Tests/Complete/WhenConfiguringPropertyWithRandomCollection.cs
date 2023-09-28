using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenConfiguringPropertyWithRandomCollection {

   public WhenConfiguringPropertyWithRandomCollection()
      => _session = AutoPocoContainer.Configure(x => {
         x.Conventions(c => c.UseDefaultConventions());
         x.Include<SimpleNode>()
           .Setup(y => y.Children).Collection(0, 3);
      })
      .CreateSession();

   private readonly IGenerationSession _session;

   [Fact]
   public void CollectionIsSetWithValidNumberInIt() {
      for (var x = 0; x < 10; x++) {
         var node = _session.Next<SimpleNode>();
         node.Children.Count.Should().BeInRange(0, 3);
      }
   }
}