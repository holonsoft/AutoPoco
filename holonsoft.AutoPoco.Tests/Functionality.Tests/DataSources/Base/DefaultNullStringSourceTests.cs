using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class DefaultNullStringSourceTests {

   [Fact]
   public void NextReturnsNullString() {
      var source = new DefaultNullStringSource();
      var value = source.Next(null);
      value.Should().BeNull();

   }
}