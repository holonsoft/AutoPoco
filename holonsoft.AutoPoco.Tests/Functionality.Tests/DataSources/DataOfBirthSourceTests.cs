using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DataOfBirthSourceTests {
   [Fact]
   public void NextReturnsAnAgeBetweenMinAndMax() {
      var source = new DateOfBirthSource(15, 18);
      var value = source.Next(null);

      value.Year.Should().BeInRange(DateTime.Now.AddYears(-18).Year, DateTime.Now.AddYears(-15).Year);
   }
}
