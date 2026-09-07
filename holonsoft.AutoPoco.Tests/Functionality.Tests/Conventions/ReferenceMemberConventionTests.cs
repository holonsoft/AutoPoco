using System.Collections;
using Shouldly;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Conventions;

public class ReferenceMemberConventionTests {
   private readonly ReferenceMemberConvention _convention = new();

   public class Child { public string? Name { get; set; } }

   public interface IChild { string? Name { get; } }

   public class Poco {
      public Child? Reference { get; set; }
      public Child? ReferenceField;
   }

   private TypeMemberConventionRequirements Requirements() {
      var requirements = new TypeMemberConventionRequirements();
      _convention.SpecifyRequirements(requirements);
      return requirements;
   }

   [Theory]
   [InlineData(typeof(Child), true)]
   [InlineData(typeof(object), true)]
   [InlineData(typeof(string), false)]        // a class, but IEnumerable
   [InlineData(typeof(List<Child>), false)]   // collections are handled elsewhere
   [InlineData(typeof(Child[]), false)]
   [InlineData(typeof(ArrayList), false)]
   [InlineData(typeof(int), false)]           // value type
   [InlineData(typeof(DateTime), false)]
   [InlineData(typeof(IChild), false)]        // interfaces are not classes
   public void RequirementsAcceptOnlyNonEnumerableClasses(Type type, bool expected)
      => Requirements().IsValidType(type).ShouldBe(expected);

   [Fact]
   public void RequirementsDoNotRestrictTheName()
      => Requirements().IsValidName("Whatever").ShouldBeTrue();

   [Fact]
   public void ApplySetsAutoSourceOfThePropertyType() {
      var context = new Mock<ITypePropertyConventionContext>();
      context.SetupGet(x => x.Member).Returns(new EngineTypePropertyMember(typeof(Poco).GetProperty(nameof(Poco.Reference))!));
      _convention.Apply(context.Object);
      context.Verify(x => x.SetSource(typeof(AutoSource<Child>)), Times.Once());
   }

   [Fact]
   public void ApplySetsAutoSourceOfTheFieldType() {
      var context = new Mock<ITypeFieldConventionContext>();
      context.SetupGet(x => x.Member).Returns(new EngineTypeFieldMember(typeof(Poco).GetField(nameof(Poco.ReferenceField))!));
      _convention.Apply(context.Object);
      context.Verify(x => x.SetSource(typeof(AutoSource<Child>)), Times.Once());
   }
}
