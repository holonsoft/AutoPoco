using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Country;

public class StatesOfACountrySourceTests : TestBase {
   [Fact]
   public void NextReturnsAState() {
      NextReturnsACountryState<GermanStatesSource>(2);
      NextReturnsACountryState<DutchStatesSource>(2);
      NextReturnsACountryState<USStatesSource>(2);
   }

   private static void NextReturnsACountryState<T>(int lengthOfAbbreviationCode)
      where T : DictionarySourceBase, new() {

      var source = new T();
      var value = source.Next(null);

      value.Should().NotBeNullOrWhiteSpace();
      value.Length.Should().BeGreaterThan(lengthOfAbbreviationCode);

      source = (T) Activator.CreateInstance(typeof(T), new object[] { true })!;
      value = source.Next(null);
      value.Should().NotBeNullOrWhiteSpace();
      value.Should().HaveLength(lengthOfAbbreviationCode);
   }

   [Fact]
   public void NextReturnsStableStateListInTermsOfTestability() {
      NextReturnsStableElementListInTermsOfTestability(
         new GermanStatesSource(),
         "Brandenburg", "Bayern", "Hamburg", "Schleswig-Holstein", "Sachsen-Anhalt", "Thüringen", "Thüringen", "Brandenburg", "Hessen", "Bremen", "Bremen",
         "Bremen", "Sachsen", "Baden-Württemberg", "Sachsen-Anhalt", "Baden-Württemberg", "Schleswig-Holstein", "Schleswig-Holstein", "Sachsen-Anhalt",
         "Baden-Württemberg"
         );

      NextReturnsStableElementListInTermsOfTestability(
         new DutchStatesSource(),
         "Friesland", "Flevoland", "Gelderland", "Zuid-Holland", "Zeeland", "Zuid-Holland", "Zuid-Holland", "Friesland", "Groningen", "Gelderland",
         "Gelderland", "Gelderland", "Utrecht", "Drenthe", "Zeeland", "Drenthe", "Zeeland", "Zuid-Holland", "Zeeland", "Drenthe"
         );

      NextReturnsStableElementListInTermsOfTestability(
         new USStatesSource(),
         "Idaho", "Connecticut", "Louisiana", "Guam", "West Virginia", "U.S. Virgin Islands", "U.S. Virgin Islands", "Hawaii", "Michigan", "Kentucky",
         "Kentucky", "Kansas", "Texas", "Arizona", "West Virginia", "Alabama", "Wyoming", "Guam", "Wisconsin", "Alabama"
         );
   }

   [Fact]
   public void NextReturnsStableStateListInTermsOfTestabilityAndListCanContainNull() {
      NextReturnsStableElementListInTermsOfTestability(
         new NullableGermanStatesSource()!,
         "Brandenburg", "Bayern", "Hamburg", "Schleswig-Holstein", "Sachsen-Anhalt", "Thüringen", "Thüringen", "Brandenburg", "Hessen", "Bremen", "Bremen",
         "Bremen", "Sachsen", "Baden-Württemberg", "Sachsen-Anhalt", "Baden-Württemberg", "Schleswig-Holstein", "Schleswig-Holstein",
         null, null
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullableDutchStatesSource()!,
         "Friesland", "Flevoland", "Gelderland", "Zuid-Holland", "Zeeland", "Zuid-Holland", "Zuid-Holland", "Friesland", "Groningen", "Gelderland",
         "Gelderland", "Gelderland", "Utrecht", "Drenthe", "Zeeland", "Drenthe", "Zeeland", "Zuid-Holland", null, null
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullableUSStatesSource()!,
         "Idaho", "Connecticut", "Louisiana", "Guam", "West Virginia", "U.S. Virgin Islands", "U.S. Virgin Islands", "Hawaii", "Michigan", "Kentucky",
         "Kentucky", "Kansas", "Texas", "Arizona", "West Virginia", "Alabama", "Wyoming", "Guam", null, null
         );
   }
}
