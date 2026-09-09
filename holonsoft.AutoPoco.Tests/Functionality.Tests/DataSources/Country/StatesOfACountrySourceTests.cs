using Shouldly;
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

      value.ShouldNotBeNullOrWhiteSpace();
      value.Length.ShouldBeGreaterThan(lengthOfAbbreviationCode);

      source = (T) Activator.CreateInstance(typeof(T), new object[] { true })!;
      value = source.Next(null);
      value.ShouldNotBeNullOrWhiteSpace();
      value.Length.ShouldBe(lengthOfAbbreviationCode);
   }

   [Fact]
   public void NextReturnsStableStateListInTermsOfTestability() {
      NextReturnsStableElementListInTermsOfTestability(
         new GermanStatesSource(), new string[] { "Sachsen", "Thüringen", "Brandenburg", "Bremen", "Brandenburg", "Sachsen-Anhalt", "Bayern", "Brandenburg", "Niedersachsen", "Baden-Württemberg", "Mecklenburg-Vorpommern", "Sachsen", "Berlin", "Berlin", "Sachsen", "Hessen", "Rheinland-Pfalz", "Bayern", "Niedersachsen", "Baden-Württemberg" });

      NextReturnsStableElementListInTermsOfTestability(
         new DutchStatesSource(), new string[] { "Gelderland", "Groningen", "Gelderland", "Flevoland", "Gelderland", "Overijssel", "Drenthe", "Noord-Holland", "Friesland", "Friesland", "Noord-Brabant", "Zeeland", "Flevoland", "Overijssel", "Drenthe", "Utrecht", "Noord-Brabant", "Gelderland", "Zuid-Holland", "Noord-Brabant" });

      NextReturnsStableElementListInTermsOfTestability(
         new USStatesSource(), new string[] { "New Hampshire", "West Virginia", "Arkansas", "Massachusetts", "Puerto Rico", "Virginia", "Wyoming", "Maryland", "North Carolina", "Mississippi", "New Hampshire", "Arizona", "Ohio", "Vermont", "Rhode Island", "Louisiana", "South Dakota", "Wisconsin", "Tennessee", "Connecticut" });
   }

   [Fact]
   public void NextReturnsStableStateListInTermsOfTestabilityAndListCanContainNull() {
      NextReturnsStableElementListInTermsOfTestability(
         new NullableGermanStatesSource()!, new string[] { "Sachsen", "Thüringen", "Brandenburg", "Bremen", "Brandenburg", "Sachsen-Anhalt", "Bayern", "Brandenburg", "Niedersachsen", "Baden-Württemberg", "Mecklenburg-Vorpommern", "Sachsen", "Berlin", "Berlin", "Sachsen", "Hessen", "Rheinland-Pfalz", "Bayern", "Niedersachsen", "Baden-Württemberg" });

      NextReturnsStableElementListInTermsOfTestability(
         new NullableDutchStatesSource()!, new string[] { "Gelderland", "Groningen", "Gelderland", "Flevoland", "Gelderland", "Overijssel", "Drenthe", "Noord-Holland", "Friesland", "Friesland", "Noord-Brabant", "Zeeland", "Flevoland", "Overijssel", "Drenthe", "Utrecht", "Noord-Brabant", "Gelderland", "Zuid-Holland", "Noord-Brabant" });

      NextReturnsStableElementListInTermsOfTestability(
         new NullableUSStatesSource()!, new string[] { "New Hampshire", "West Virginia", "Arkansas", "Massachusetts", "Puerto Rico", "Virginia", "Wyoming", "Maryland", "North Carolina", "Mississippi", "New Hampshire", "Arizona", "Ohio", "Vermont", "Rhode Island", "Louisiana", "South Dakota", "Wisconsin", "Tennessee", "Connecticut" });
   }
}
