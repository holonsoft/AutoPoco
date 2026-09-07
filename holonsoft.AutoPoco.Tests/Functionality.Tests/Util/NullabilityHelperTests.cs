using System.Reflection;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class NullabilityHelperTests {
   private static PropertyInfo Property<T>(string name) => typeof(T).GetProperty(name)!;
   private static FieldInfo Field<T>(string name) => typeof(T).GetField(name)!;
   private static ParameterInfo Parameter(string name)
      => typeof(NullableMembersClass).GetMethod(nameof(NullableMembersClass.Method))!.GetParameters().Single(x => x.Name == name);

   [Theory]
   [InlineData(nameof(NullableMembersClass.NullableText), true)]
   [InlineData(nameof(NullableMembersClass.NonNullableText), false)]
   [InlineData(nameof(NullableMembersClass.NullableNumber), true)]
   [InlineData(nameof(NullableMembersClass.NonNullableNumber), false)]
   [InlineData(nameof(NullableMembersClass.NullableDate), true)]
   [InlineData(nameof(NullableMembersClass.NullableReference), true)]
   [InlineData(nameof(NullableMembersClass.AllowNullText), true)]
   [InlineData(nameof(NullableMembersClass.DisallowNullText), false)]
   [InlineData(nameof(NullableMembersClass.GetOnlyNullable), true)]
   [InlineData(nameof(NullableMembersClass.GetOnlyNonNullable), false)]
   public void PropertyNullabilityFollowsTheAnnotation(string name, bool expected)
      => NullabilityHelper.AllowsNull(Property<NullableMembersClass>(name)).ShouldBe(expected);

   [Theory]
   [InlineData(nameof(SimpleUser.FirstName), false)]
   [InlineData(nameof(SimpleUser.City), true)]
   [InlineData(nameof(SimpleUser.Birthday), false)]
   [InlineData(nameof(SimpleUser.Properties), false)]
   public void RequiredAndAnnotatedMembersOfARealModelAreRecognized(string name, bool expected)
      => NullabilityHelper.AllowsNull(Property<SimpleUser>(name)).ShouldBe(expected);

   [Theory]
   [InlineData(nameof(NullableMembersClass.NullableField), true)]
   [InlineData(nameof(NullableMembersClass.NonNullableField), false)]
   [InlineData(nameof(NullableMembersClass.NullableNumberField), true)]
   public void FieldNullabilityFollowsTheAnnotation(string name, bool expected)
      => NullabilityHelper.AllowsNull(Field<NullableMembersClass>(name)).ShouldBe(expected);

   [Theory]
   [InlineData("nullableParameter", true)]
   [InlineData("nonNullableParameter", false)]
   [InlineData("nullableNumber", true)]
   [InlineData("number", false)]
   public void ParameterNullabilityFollowsTheAnnotation(string name, bool expected)
      => NullabilityHelper.AllowsNull(Parameter(name)).ShouldBe(expected);

   [Fact]
   public void ObliviousReferenceMembersAreNotNullable() {
      NullabilityHelper.AllowsNull(Property<ObliviousMembersClass>(nameof(ObliviousMembersClass.Text))).ShouldBeFalse();
      NullabilityHelper.AllowsNull(Field<ObliviousMembersClass>(nameof(ObliviousMembersClass.Field))).ShouldBeFalse();
   }

   [Fact]
   public void ObliviousNullableValueTypeIsStillNullable()
      => NullabilityHelper.AllowsNull(Property<ObliviousMembersClass>(nameof(ObliviousMembersClass.Number))).ShouldBeTrue();

   [Fact]
   public void NullArgumentsThrow() {
      Should.Throw<ArgumentNullException>(() => NullabilityHelper.AllowsNull((PropertyInfo) null!));
      Should.Throw<ArgumentNullException>(() => NullabilityHelper.AllowsNull((FieldInfo) null!));
      Should.Throw<ArgumentNullException>(() => NullabilityHelper.AllowsNull((ParameterInfo) null!));
   }
}
