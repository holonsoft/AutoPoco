using Xunit;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenSettingALimitOnRecursiveTrees {

   public WhenSettingALimitOnRecursiveTrees()
      => _session = AutoPocoContainer
            .Configure(x => {
               x.Conventions(c => { c.UseDefaultConventions(); });
               x.Include<SimpleNode>()
               .Setup(y => y.Parent).Default();
            })
            .CreateSession(10);

   private readonly IGenerationSession _session;

   [Fact]
   public void RequestingRecursiveTypeHonoursLengthLimit() {
      var node = _session.Next<SimpleNode>();
      var x = 0;
      while (node != null) {
         node = node.Parent;
         x++;

         if (x > 10) {
            Assert.Fail("Recursive limit was not honoured");
            break;
         }
      }
   }
}