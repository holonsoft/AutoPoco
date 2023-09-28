using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class ReflectionHelperTests {
   [Fact]
   public void GetPropertyReturnsPropertyInfo() {
      var info = ReflectionHelper.GetProperty<SimplePropertyClass>(x => x.SomeProperty!);

      info.Name.Should().Be("SomeProperty");
   }

   [Fact]
   public void GetMemberAsPropertyReturnsMember() {
      var member = ReflectionHelper.GetMember<SimplePropertyClass>(x => x.SomeProperty!);
      member.Name.Should().Be("SomeProperty");
      member.IsProperty.Should().BeTrue();
   }

   [Fact]
   public void GetMemberAsFieldReturnsField() {
      var member = ReflectionHelper.GetMember<SimpleFieldClass>(x => x.SomeField!);
      member.Name.Should().Be("SomeField");
      member.IsField.Should().BeTrue();
   }

   [Fact]
   public void GetFieldReturnsFieldInfo() {
      var info = ReflectionHelper.GetField<SimpleFieldClass>(x => x.SomeField!);

      info.Name.Should().Be("SomeField");
   }
}