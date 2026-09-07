using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

/// <summary>
///   Shared fixtures for the default member conventions (string, int, float, DateTime).
/// </summary>
public static class DefaultMemberConventionsTestSupport {
   public class Poco {
      public string? Text { get; set; }
      public int Number { get; set; }
      public float Ratio { get; set; }
      public DateTime When { get; set; }
      public double Other { get; set; }

      public string? TextField;
      public int NumberField;
      public float RatioField;
      public DateTime WhenField;
      public double OtherField;
   }

   public static Mock<ITypePropertyConventionContext> PropertyContext(string propertyName) {
      var context = new Mock<ITypePropertyConventionContext>();
      context.SetupGet(x => x.Member).Returns(new EngineTypePropertyMember(typeof(Poco).GetProperty(propertyName)!));
      return context;
   }

   public static Mock<ITypeFieldConventionContext> FieldContext(string fieldName) {
      var context = new Mock<ITypeFieldConventionContext>();
      context.SetupGet(x => x.Member).Returns(new EngineTypeFieldMember(typeof(Poco).GetField(fieldName)!));
      return context;
   }

   public static TypeMemberConventionRequirements RequirementsOf(Action<ITypeMemberConventionRequirements> specify) {
      var requirements = new TypeMemberConventionRequirements();
      specify(requirements);
      return requirements;
   }
}

public class DefaultStringMemberConventionTests {
   private readonly DefaultStringMemberConvention _convention = new();

   [Theory]
   [InlineData(typeof(string), true)]
   [InlineData(typeof(int), false)]
   [InlineData(typeof(object), false)]
   [InlineData(typeof(char[]), false)]
   public void RequirementsAcceptOnlyString(Type type, bool expected)
      => DefaultMemberConventionsTestSupport.RequirementsOf(_convention.SpecifyRequirements).IsValidType(type).ShouldBe(expected);

   [Fact]
   public void RequirementsDoNotRestrictTheName()
      => DefaultMemberConventionsTestSupport.RequirementsOf(_convention.SpecifyRequirements).IsValidName("Anything").ShouldBeTrue();

   [Fact]
   public void ApplySetsEmptyStringOnStringProperty() {
      var context = DefaultMemberConventionsTestSupport.PropertyContext("Text");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(""), Times.Once());
   }

   [Fact]
   public void ApplySetsEmptyStringOnStringField() {
      var context = DefaultMemberConventionsTestSupport.FieldContext("TextField");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(""), Times.Once());
   }

   [Fact]
   public void ApplyIgnoresPropertyOfOtherType() {
      var context = DefaultMemberConventionsTestSupport.PropertyContext("Other");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
   }

   [Fact]
   public void ApplyIgnoresFieldOfOtherType() {
      var context = DefaultMemberConventionsTestSupport.FieldContext("OtherField");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
   }
}

public class DefaultIntegerMemberConventionTests {
   private readonly DefaultIntegerMemberConvention _convention = new();

   [Theory]
   [InlineData(typeof(int), true)]
   [InlineData(typeof(long), false)]
   [InlineData(typeof(short), false)]
   [InlineData(typeof(int?), false)]
   [InlineData(typeof(string), false)]
   public void RequirementsAcceptOnlyInt32(Type type, bool expected)
      => DefaultMemberConventionsTestSupport.RequirementsOf(_convention.SpecifyRequirements).IsValidType(type).ShouldBe(expected);

   [Fact]
   public void ApplySetsZeroOnIntProperty() {
      var context = DefaultMemberConventionsTestSupport.PropertyContext("Number");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(0), Times.Once());
   }

   [Fact]
   public void ApplySetsZeroOnIntField() {
      var context = DefaultMemberConventionsTestSupport.FieldContext("NumberField");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(0), Times.Once());
   }

   [Fact]
   public void ApplyIgnoresMembersOfOtherType() {
      var property = DefaultMemberConventionsTestSupport.PropertyContext("Other");
      var field = DefaultMemberConventionsTestSupport.FieldContext("OtherField");
      _convention.Apply(property.Object);
      _convention.Apply(field.Object);
      property.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
      field.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
   }
}

public class DefaultFloatMemberConventionTests {
   private readonly DefaultFloatMemberConvention _convention = new();

   [Theory]
   [InlineData(typeof(float), true)]
   [InlineData(typeof(double), false)]
   [InlineData(typeof(decimal), false)]
   [InlineData(typeof(float?), false)]
   public void RequirementsAcceptOnlySingle(Type type, bool expected)
      => DefaultMemberConventionsTestSupport.RequirementsOf(_convention.SpecifyRequirements).IsValidType(type).ShouldBe(expected);

   [Fact]
   public void ApplySetsZeroOnFloatProperty() {
      object? captured = null;
      var context = DefaultMemberConventionsTestSupport.PropertyContext("Ratio");
      context.Setup(x => x.SetValue(It.IsAny<object>())).Callback((object value) => captured = value);
      _convention.Apply(context.Object);
      captured.ShouldNotBeNull();
      Convert.ToSingle(captured).ShouldBe(0f);
   }

   [Fact]
   public void ApplySetsZeroOnFloatField() {
      object? captured = null;
      var context = DefaultMemberConventionsTestSupport.FieldContext("RatioField");
      context.Setup(x => x.SetValue(It.IsAny<object>())).Callback((object value) => captured = value);
      _convention.Apply(context.Object);
      captured.ShouldNotBeNull();
      Convert.ToSingle(captured).ShouldBe(0f);
   }

   [Fact]
   public void ApplyIgnoresMembersOfOtherType() {
      var property = DefaultMemberConventionsTestSupport.PropertyContext("Other");
      var field = DefaultMemberConventionsTestSupport.FieldContext("OtherField");
      _convention.Apply(property.Object);
      _convention.Apply(field.Object);
      property.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
      field.Verify(x => x.SetValue(It.IsAny<object>()), Times.Never());
   }
}

public class DefaultDatetimeMemberConventionTests {
   private readonly DefaultDatetimeMemberConvention _convention = new();

   [Theory]
   [InlineData(typeof(DateTime), true)]
   [InlineData(typeof(DateTime?), false)]
   [InlineData(typeof(DateOnly), false)]
   [InlineData(typeof(DateTimeOffset), false)]
   [InlineData(typeof(string), false)]
   public void RequirementsAcceptOnlyDateTime(Type type, bool expected)
      => DefaultMemberConventionsTestSupport.RequirementsOf(_convention.SpecifyRequirements).IsValidType(type).ShouldBe(expected);

   [Fact]
   public void ApplySetsMinValueOnDateTimeProperty() {
      var context = DefaultMemberConventionsTestSupport.PropertyContext("When");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(DateTime.MinValue), Times.Once());
   }

   [Fact]
   public void ApplySetsMinValueOnDateTimeField() {
      var context = DefaultMemberConventionsTestSupport.FieldContext("WhenField");
      _convention.Apply(context.Object);
      context.Verify(x => x.SetValue(DateTime.MinValue), Times.Once());
   }
}
