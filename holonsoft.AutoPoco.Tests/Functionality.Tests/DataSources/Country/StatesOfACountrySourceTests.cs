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
         "Brandenburg", null, "Bayern", "Hamburg", "Schleswig-Holstein", "Sachsen-Anhalt", "Thüringen", "Thüringen", "Brandenburg", "Hessen",
         "Bremen", "Bremen", "Bremen", null, "Sachsen", null, "Baden-Württemberg", "Sachsen-Anhalt", "Baden-Württemberg", null
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullableDutchStatesSource()!,
         "Friesland", null, "Flevoland", "Gelderland", "Zuid-Holland", "Zeeland", "Zuid-Holland", "Zuid-Holland", "Friesland", "Groningen",
         "Gelderland", "Gelderland", "Gelderland", null, "Utrecht", null, "Drenthe", "Zeeland", "Drenthe", null
         );

      NextReturnsStableElementListInTermsOfTestability(
         new NullableUSStatesSource()!,
         "Idaho", null, "Connecticut", "Louisiana", "Guam", "West Virginia", "U.S. Virgin Islands", "U.S. Virgin Islands", "Hawaii", "Michigan",
         "Kentucky", "Kentucky", "Kansas", null, "Texas", null, "Arizona", "West Virginia", "Alabama", null
         );
   }
}
