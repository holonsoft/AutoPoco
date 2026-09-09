using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.DataSources.Country;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

/// <summary>
///   Regression for 6.0: every Nullable* source used to ignore an explicit null creation threshold
///   (the fixed array and dictionary sources even used it as the random seed).
/// </summary>
public class NullCreationThresholdTests {
   private static readonly Dictionary<string, Func<int, DataSourceBase<string>>> _factories = new() {
      ["FirstName"] = t => new NullableFirstNameSource(t),
      ["LastName"] = t => new NullableLastNameSource(t),
      ["Company"] = t => new NullableCompanySource(t),
      ["Url"] = t => new NullableUrlSource(t),
      ["Capital"] = t => new NullableCapitalSource(t),
      ["City"] = t => new NullableCitySource(t),
      ["PostalZipCodeGermany"] = t => new NullablePostalZipCodeGermanySource(t),
      ["PostalZipCodeNetherlands"] = t => new NullablePostalZipCodeNetherlandsSource(t),
      ["PostalZipCodeUSA"] = t => new NullablePostalZipCodeUSASource(t),
      ["Country"] = t => new NullableCountrySource(false, t),
      ["CountryAbbreviation"] = t => new NullableCountrySource(true, t),
      ["GermanStates"] = t => new NullableGermanStatesSource(false, t),
      ["DutchStates"] = t => new NullableDutchStatesSource(false, t),
      ["UsStates"] = t => new NullableUSStatesSource(false, t),
      ["EmailAddress"] = t => new NullableEmailAddressSource(t),
      ["ExtendedEmailAddress"] = t => new NullableExtendedEmailAddressSource(t),
      ["LoremIpsum"] = t => new NullableLoremIpsumSource(1, t),
      ["CreditCard"] = t => new NullableCreditCardSource(t),
      ["RandomText"] = t => new NullableRandomTextSource(t, 500, 3, 6, 3, 7, "abcdefghijklmnopqrstuvwxyz".ToCharArray()),
      ["RandomUtfText"] = t => new NullableRandomUtfTextSource(t, 500, 3, 6, 3, 7),
      ["RandomString"] = t => new NullableRandomStringSource(3, 5, 'a', 'z', t)!,
   };

   private static readonly Dictionary<string, Func<DataSourceBase<string>>> _defaultFactories = new() {
      ["FirstName"] = () => new NullableFirstNameSource(),
      ["Company"] = () => new NullableCompanySource(),
      ["Country"] = () => new NullableCountrySource(),
      ["EmailAddress"] = () => new NullableEmailAddressSource(),
      ["LoremIpsum"] = () => new NullableLoremIpsumSource(),
      ["CreditCard"] = () => new NullableCreditCardSource(),
      ["RandomText"] = () => new NullableRandomTextSource(),
      ["RandomUtfText"] = () => new NullableRandomUtfTextSource(),
      ["RandomString"] = () => new NullableRandomStringSource()!,
   };

   public static TheoryData<string> SourceNames => new(_factories.Keys);

   public static TheoryData<string> DefaultSourceNames => new(_defaultFactories.Keys);

   private static List<string?> Draw(DataSourceBase<string> source, int count)
      => Enumerable.Range(0, count).Select(_ => (string?) source.Next(null)).ToList();

   [Theory]
   [MemberData(nameof(SourceNames))]
   public void ThresholdOf100AlwaysReturnsNull(string name)
      => Draw(_factories[name](100), 30).ShouldAllBe(x => x == null);

   [Theory]
   [MemberData(nameof(SourceNames))]
   public void ThresholdOf0NeverReturnsNull(string name)
      => Draw(_factories[name](0), 30).ShouldAllBe(x => x != null);

   [Theory]
   [MemberData(nameof(SourceNames))]
   public void ThresholdOf50ReturnsAMixOfNullsAndValues(string name) {
      var values = Draw(_factories[name](50), 100);

      values.ShouldContain(x => x == null);
      values.ShouldContain(x => x != null);
   }

   [Theory]
   [MemberData(nameof(SourceNames))]
   public void ThresholdIsAppliedToTheEvaluator(string name)
      => _factories[name](37).RandomNullEvaluator.ShouldBeOfType<DefaultRandomNullEvaluator>().ThresholdPercentage.ShouldBe(37);

   [Theory]
   [MemberData(nameof(DefaultSourceNames))]
   public void DefaultConstructorUsesTheGlobalThreshold(string name)
      => _defaultFactories[name]().RandomNullEvaluator.ShouldBeOfType<DefaultRandomNullEvaluator>()
         .ThresholdPercentage.ShouldBe(AutoPocoDefaults.NullCreationThreshold);

   [Fact]
   public void ThresholdDoesNotChangeTheDataSequence() {
      var plain = Draw(new FirstNameSource(), 30);
      var nullable = Draw(new NullableFirstNameSource(0), 30);

      nullable.ShouldBe(plain);
   }

   [Fact]
   public void NonNullableSourcesNeverReturnNullEvenWithAThresholdOnTheEvaluator() {
      var source = new FirstNameSource();
      source.SetNullCreationThreshold(100);

      Draw(source, 30).ShouldAllBe(x => x != null);
   }

   [Fact]
   public void ACustomEvaluatorIsHonored() {
      var source = new NullableFirstNameSource(0);
      source.SetRandomNullEvaluator(new AlwaysNullEvaluator());

      Draw(source, 10).ShouldAllBe(x => x == null);
   }

   private sealed class AlwaysNullEvaluator : IRandomNullEvaluator {
      public bool ShouldNextValueReturnNull() => true;
      public void SetSeedToRandomValue(int seed) { }
   }
}
