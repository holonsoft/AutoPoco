using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Business;

public class ExtendedEmailAddressSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new ExtendedEmailAddressSource(), new string[] { "Ethan.Brown@heise.example", "Brian.Mason@yahoo.test", "Mila.Ortiz@hotmail.invalid", "Robert.Lopez@google.example", "Ellie.Murphy@golem.test", "Michael.Patterson@hotmail.test", "Sophie.Mason@microsoft.test", "Hannah.Fox@golem.test", "Isaac.Parker@aol.example", "Ariana.Walker@google.invalid" });

   [Fact]
   public void NextReturnsStableEmailListInTermsOfTestabilityAndListCanContainNull() => NextReturnsStableElementListInTermsOfTestability(
         new NullableExtendedEmailAddressSource()!, new string[] { "Ethan.Brown@heise.example", "Brian.Mason@yahoo.test", "Mila.Ortiz@hotmail.invalid", "Robert.Lopez@google.example", "Ellie.Murphy@golem.test", "Michael.Patterson@hotmail.test", "Sophie.Mason@microsoft.test", "Hannah.Fox@golem.test", "Isaac.Parker@aol.example", "Ariana.Walker@google.invalid" });

   [Fact]
   public void TheFirstAndTheLastNameDoNotFollowTheSameStream() {
      // both nested sources used to share one seed, so the same first name came back with the same last name
      var source = new ExtendedEmailAddressSource();
      var pairs = Enumerable.Range(0, 200).Select(_ => source.Next(null).Split('@')[0]).ToList();
      var firstNames = pairs.Select(p => p.Split('.')[0]).ToList();

      var repeatedFirstName = firstNames
         .GroupBy(x => x)
         .FirstOrDefault(g => g.Count() > 1)?.Key;

      repeatedFirstName.ShouldNotBeNull("the sample is too small to say anything about the pairing");
      pairs.Where(p => p.StartsWith($"{repeatedFirstName}.", StringComparison.Ordinal))
         .Distinct()
         .Count()
         .ShouldBeGreaterThan(1);
   }
}