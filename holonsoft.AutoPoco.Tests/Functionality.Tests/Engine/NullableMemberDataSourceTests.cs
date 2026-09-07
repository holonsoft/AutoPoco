using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Engine;

public class NullableMemberDataSourceTests {
   private sealed class CountingSource : IDataSource {
      public int Calls { get; private set; }
      public IGenerationContext? SeenContext { get; private set; }

      public object? InternalNext(IGenerationContext? context) {
         Calls++;
         SeenContext = context;
         return "value";
      }
   }

   private sealed class FixedEvaluator(bool result) : IRandomNullEvaluator {
      public bool ShouldNextValueReturnNull() => result;
      public void SetSeedToRandomValue(int seed) { }
   }

   private static EngineTypePropertyMember Property(string name)
      => new(typeof(NullableMembersClass).GetProperty(name)!);

   private static EngineTypeFieldMember Field(string name)
      => new(typeof(NullableMembersClass).GetField(name)!);

   private static List<bool> Draw(IRandomNullEvaluator evaluator, int count)
      => Enumerable.Range(0, count).Select(_ => evaluator.ShouldNextValueReturnNull()).ToList();

   [Fact]
   public void ConstructorRejectsNullArguments() {
      Should.Throw<ArgumentNullException>(() => new NullableMemberDataSource(null!, new FixedEvaluator(false)));
      Should.Throw<ArgumentNullException>(() => new NullableMemberDataSource(new CountingSource(), null!));
   }

   [Fact]
   public void ReturnsNullWithoutAskingTheInnerSourceWhenTheEvaluatorSaysNull() {
      var inner = new CountingSource();
      var source = new NullableMemberDataSource(inner, new FixedEvaluator(true));

      source.InternalNext(null).ShouldBeNull();
      inner.Calls.ShouldBe(0);
   }

   [Fact]
   public void DelegatesToTheInnerSourceAndPassesTheContextOtherwise() {
      var inner = new CountingSource();
      var context = new Mock<IGenerationContext>().Object;
      var source = new NullableMemberDataSource(inner, new FixedEvaluator(false));

      source.InternalNext(context).ShouldBe("value");
      inner.Calls.ShouldBe(1);
      inner.SeenContext.ShouldBeSameAs(context);
   }

   [Fact]
   public void ExposesInnerSourceAndEvaluator() {
      var inner = new CountingSource();
      var evaluator = new FixedEvaluator(false);
      var source = new NullableMemberDataSource(inner, evaluator);

      source.Inner.ShouldBeSameAs(inner);
      source.RandomNullEvaluator.ShouldBeSameAs(evaluator);
   }

   [Fact]
   public void ForMemberRejectsNullArguments() {
      Should.Throw<ArgumentNullException>(() => NullableMemberDataSource.ForMember(null!, Property(nameof(NullableMembersClass.NullableText)), NullableAnnotationSettings.Enabled()));
      Should.Throw<ArgumentNullException>(() => NullableMemberDataSource.ForMember(new CountingSource(), null!, NullableAnnotationSettings.Enabled()));
   }

   [Fact]
   public void ForMemberReturnsTheSourceUnchangedWhenSettingsAreMissingOrDisabled() {
      var inner = new CountingSource();
      var member = Property(nameof(NullableMembersClass.NullableText));

      NullableMemberDataSource.ForMember(inner, member, null).ShouldBeSameAs(inner);
      NullableMemberDataSource.ForMember(inner, member, NullableAnnotationSettings.Disabled).ShouldBeSameAs(inner);
   }

   [Theory]
   [InlineData(nameof(NullableMembersClass.NonNullableText))]
   [InlineData(nameof(NullableMembersClass.NonNullableNumber))]
   [InlineData(nameof(NullableMembersClass.DisallowNullText))]
   public void ForMemberReturnsTheSourceUnchangedForANonNullableProperty(string name) {
      var inner = new CountingSource();

      NullableMemberDataSource.ForMember(inner, Property(name), NullableAnnotationSettings.Enabled()).ShouldBeSameAs(inner);
   }

   [Theory]
   [InlineData(nameof(NullableMembersClass.NullableText))]
   [InlineData(nameof(NullableMembersClass.NullableNumber))]
   [InlineData(nameof(NullableMembersClass.NullableDate))]
   [InlineData(nameof(NullableMembersClass.NullableReference))]
   [InlineData(nameof(NullableMembersClass.AllowNullText))]
   public void ForMemberWrapsANullablePropertyWithTheConfiguredThreshold(string name) {
      var inner = new CountingSource();

      var result = NullableMemberDataSource.ForMember(inner, Property(name), NullableAnnotationSettings.Enabled(42));

      var wrapped = result.ShouldBeOfType<NullableMemberDataSource>();
      wrapped.Inner.ShouldBeSameAs(inner);
      wrapped.RandomNullEvaluator.ShouldBeOfType<DefaultRandomNullEvaluator>().ThresholdPercentage.ShouldBe(42);
   }

   [Fact]
   public void ForMemberWrapsANullableFieldAndLeavesANonNullableFieldAlone() {
      var inner = new CountingSource();

      NullableMemberDataSource.ForMember(inner, Field(nameof(NullableMembersClass.NullableField)), NullableAnnotationSettings.Enabled())
         .ShouldBeOfType<NullableMemberDataSource>();
      NullableMemberDataSource.ForMember(inner, Field(nameof(NullableMembersClass.NullableNumberField)), NullableAnnotationSettings.Enabled())
         .ShouldBeOfType<NullableMemberDataSource>();
      NullableMemberDataSource.ForMember(inner, Field(nameof(NullableMembersClass.NonNullableField)), NullableAnnotationSettings.Enabled())
         .ShouldBeSameAs(inner);
   }

   [Fact]
   public void ForMemberNeverWrapsAMethodMember() {
      var inner = new CountingSource();
      var member = new EngineTypeMethodMember(typeof(NullableMembersClass).GetMethod(nameof(NullableMembersClass.Method))!);

      NullableMemberDataSource.ForMember(inner, member, NullableAnnotationSettings.Enabled(100)).ShouldBeSameAs(inner);
   }

   [Fact]
   public void ForMemberLeavesObliviousReferenceMembersAlone() {
      var inner = new CountingSource();
      var member = new EngineTypePropertyMember(typeof(ObliviousMembersClass).GetProperty(nameof(ObliviousMembersClass.Text))!);

      NullableMemberDataSource.ForMember(inner, member, NullableAnnotationSettings.Enabled(100)).ShouldBeSameAs(inner);
   }

   [Fact]
   public void DifferentMembersGetDifferentNullPatterns() {
      var settings = NullableAnnotationSettings.Enabled(50);
      var text = (NullableMemberDataSource) NullableMemberDataSource.ForMember(new CountingSource(), Property(nameof(NullableMembersClass.NullableText)), settings);
      var number = (NullableMemberDataSource) NullableMemberDataSource.ForMember(new CountingSource(), Property(nameof(NullableMembersClass.NullableNumber)), settings);

      Draw(text.RandomNullEvaluator, 64).ShouldNotBe(Draw(number.RandomNullEvaluator, 64));
   }

   [Fact]
   public void TheSameMemberAlwaysGetsTheSameNullPattern() {
      var settings = NullableAnnotationSettings.Enabled(50);
      var first = (NullableMemberDataSource) NullableMemberDataSource.ForMember(new CountingSource(), Property(nameof(NullableMembersClass.NullableText)), settings);
      var second = (NullableMemberDataSource) NullableMemberDataSource.ForMember(new CountingSource(), Property(nameof(NullableMembersClass.NullableText)), settings);

      Draw(first.RandomNullEvaluator, 64).ShouldBe(Draw(second.RandomNullEvaluator, 64));
   }
}
