using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class ReflectionHelperTests {
   [Fact]
   public void GetPropertyReturnsPropertyInfo() {
      var info = ReflectionHelper.GetProperty<SimplePropertyClass>(x => x.SomeProperty!);

      info.Name.ShouldBe("SomeProperty");
   }

   [Fact]
   public void GetMemberAsPropertyReturnsMember() {
      var member = ReflectionHelper.GetMember<SimplePropertyClass>(x => x.SomeProperty!);
      member.Name.ShouldBe("SomeProperty");
      member.IsProperty.ShouldBeTrue();
   }

   [Fact]
   public void GetMemberAsFieldReturnsField() {
      var member = ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!);
      member.Name.ShouldBe("SomeField");
      member.IsField.ShouldBeTrue();
   }

   [Fact]
   public void GetFieldReturnsFieldInfo() {
      var info = ReflectionHelper.GetField<SimpleFieldClass>(x => x.SomeField!);

      info.Name.ShouldBe("SomeField");
   }
}