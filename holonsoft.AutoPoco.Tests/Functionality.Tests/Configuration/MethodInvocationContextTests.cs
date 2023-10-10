using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class MethodInvocationContextTests {
   [Fact]
   public void AddArgumentSourceAddsArgument() {
      var context = new MethodInvocationContext();
      context.AddArgumentSource(typeof(RandomStringSource));

      var factory = context.GetArguments().First();
      Assert.NotNull(factory);
   }

   [Fact]
   public void AddArgumentValueAddsArgument() {
      var context = new MethodInvocationContext();
      context.AddArgumentValue(5);

      var factory = context.GetArguments().First();
      Assert.NotNull(factory);
   }
}