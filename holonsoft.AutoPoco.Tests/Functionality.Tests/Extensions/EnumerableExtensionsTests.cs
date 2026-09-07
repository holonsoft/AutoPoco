using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Extensions;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Extensions;

public class EnumerableExtensionsTests {
   public class Base { }
   public class Derived : Base { }
   public interface IMarker { }

   [Fact]
   public void AncestorsAndSelfOfNullIsEmpty()
      => EnumerableExtensions.AncestorsAndSelf(null).ShouldBeEmpty();

   [Fact]
   public void AncestorsAndSelfWalksUpToObject()
      => EnumerableExtensions.AncestorsAndSelf(typeof(Derived))
         .ShouldBe([typeof(Derived), typeof(Base), typeof(object)]);

   [Fact]
   public void AncestorsAndSelfOfObjectIsObjectOnly()
      => EnumerableExtensions.AncestorsAndSelf(typeof(object)).ShouldHaveSingleItem().ShouldBe(typeof(object));

   [Fact]
   public void AncestorsAndSelfOfInterfaceIsTheInterfaceOnly()
      => EnumerableExtensions.AncestorsAndSelf(typeof(IMarker)).ShouldHaveSingleItem().ShouldBe(typeof(IMarker));

   [Fact]
   public void AncestorsAndSelfOfValueTypeEndsAtObject()
      => EnumerableExtensions.AncestorsAndSelf(typeof(int))
         .ShouldBe([typeof(int), typeof(ValueType), typeof(object)]);
}
