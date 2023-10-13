using FluentAssertions;
using Moq;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

public class TypeMemberConventionContextTests {
   private readonly TypeMemberConventionContext _context;
   private readonly Mock<IEngineConfigurationTypeMember> _memberMock;

   public TypeMemberConventionContextTests() {
      _memberMock = new Mock<IEngineConfigurationTypeMember>();
      _context = new TypeMemberConventionContext(null!, _memberMock.Object);
   }

   [Fact]
   public void SetValueSetsDatasource() {
      _context.SetValue(10);
      _memberMock.Verify(x => x.SetDataSource(It.IsAny<IEngineConfigurationDataSource>()), Times.Once());
   }

   [Fact]
   public void SetValueSetsValueDataSource() {
      IEngineConfigurationDataSource? source = null!;
      _memberMock.Setup(x => x.SetDataSource(It.IsAny<IEngineConfigurationDataSource>()))
        .Callback((IEngineConfigurationDataSource configSource) => { source = configSource; });
      _context.SetValue(10);
      var dataSource = source.Build()!;

      dataSource.InternalNext(null!).Should().Be(10);
   }

   [Fact]
   public void SetSouceSetsDatasource() {
      _context.SetSource<TestSource>();
      _memberMock.Verify(x => x.SetDataSource(It.IsAny<IEngineConfigurationDataSource>()), Times.Once());
   }

   [Fact]
   public void SetSourceSetsCorrectDatasource() {
      IEngineConfigurationDataSource? source = null!;
      _memberMock.Setup(x => x.SetDataSource(It.IsAny<IEngineConfigurationDataSource>()))
        .Callback((IEngineConfigurationDataSource configSource) => { source = configSource; });
      _context.SetSource<TestSource>();
      var dataSource = source.Build()!;
      dataSource.GetType().Should().Be(typeof(TestSource));
   }

   [Fact]
   public void MemberReturnsConfigurationMember() {
      var field = ReflectionHelper.GetMember<TestClass>(x => x.Field!);
      _memberMock.SetupGet(x => x.Member).Returns(field);
      _context.Member.Should().Be(field);
   }

   public class TestClass {
      public string? Field;
   }

   public class TestSource : IDataSource {
      public object InternalNext(IGenerationContext? context) => throw new NotImplementedException();
   }
}