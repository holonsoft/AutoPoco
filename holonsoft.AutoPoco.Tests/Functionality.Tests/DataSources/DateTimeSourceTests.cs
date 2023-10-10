using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DateTimeSourceTests {
   [Fact]
   public void NextReturnsAnDateTimeBetweenMinAndMax() {
      var minDate = new DateTime(2000, 1, 1);
      var maxDate = new DateTime(2030, 12, 31);
      var source = new DateTimeSource(minDate, maxDate);
      var value = source.Next(null);

      value.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);

      List<DateTime> generated = new();

      for (var i = 0; i < 5; i++)
         generated.Add(source.Next(null));

      generated.Should()
            .NotBeEmpty()
            .And
            .HaveCount(5)
            .And
            .ContainInOrder(new[] { new DateTime(631977283599666314), new DateTime(633903878513315964), new DateTime(639933567218770876), new DateTime(639064005583396363), new DateTime(640216204016943624), })
            .And.ContainItemsAssignableTo<DateTime>();

   }
}