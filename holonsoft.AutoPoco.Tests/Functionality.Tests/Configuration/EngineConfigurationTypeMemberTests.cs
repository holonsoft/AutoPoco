using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class EngineConfigurationTypeMemberTests {
   [Fact]
   public void GetSourceReturnsSource() {
      var member = new EngineConfigurationTypeMember(ReflectionHelper.GetMember<SimpleUser>(x => x.FirstName));
      var sourceMock = new Mock<IEngineConfigurationDataSource>();
      member.SetDataSource(sourceMock.Object);

      var source2 = member.GetDataSources().First();

      source2.Should().BeSameAs(sourceMock.Object);

      //Assert.AreEqual(sourceMock.Object, source2);
   }
}