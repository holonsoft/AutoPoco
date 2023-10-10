using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class StatesOfACountrySourceTests {
   [Fact]
   public void NextReturnsAState() {
      NextReturnsACountryState<GermanStatesSource>(2);
      NextReturnsACountryState<DutchStatesSource>(2);
      NextReturnsACountryState<UsStatesSource>(2);
   }

   private static void NextReturnsACountryState<T>(int lengthOfAbbreviationCode)
      where T : StatesSourceBase, new() {

      var source = new T();
      var value = source.Next(null);

      value.Should().NotBeNullOrWhiteSpace();
      value.Length.Should().BeGreaterThan(lengthOfAbbreviationCode);

      source = (T) Activator.CreateInstance(typeof(T), new object[] { true })!;
      value = source.Next(null);
      value.Should().NotBeNullOrWhiteSpace();
      value.Should().HaveLength(lengthOfAbbreviationCode);
   }
}
