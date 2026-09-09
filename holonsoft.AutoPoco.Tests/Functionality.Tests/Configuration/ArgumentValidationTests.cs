using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Conventions;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Business;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Configuration;

/// <summary>
///   The public surface rejects null and invalid arguments up front instead of failing somewhere inside.
/// </summary>
public class ArgumentValidationTests {
   private static IGenerationSession Session()
      => AutoPocoContainer.Configure(x => x.Include<SimpleUserRole>().Setup(r => r.Name).Use<FirstNameSource>()).CreateSession();

   [Fact]
   public void ContainerRejectsNullSetup()
      => Should.Throw<ArgumentNullException>(() => AutoPocoContainer.Configure(null!));

   [Fact]
   public void ConfigurationBuilderRejectsNulls() {
      var builder = new EngineConfigurationBuilder();

      Should.Throw<ArgumentNullException>(() => builder.Include((Type) null!));
      Should.Throw<ArgumentNullException>(() => builder.Conventions(null!));
      Should.Throw<ArgumentNullException>(() => builder.RegisterTypeProvider(null!));
   }

   [Fact]
   public void ConventionConfigurationRejectsNullAndNonConventionTypes() {
      var conventions = new EngineConventionConfiguration();

      Should.Throw<ArgumentNullException>(() => conventions.Register(null!));
      Should.Throw<ArgumentNullException>(() => conventions.ScanAssembly(null!));
      Should.Throw<ArgumentException>(() => conventions.Register(typeof(string))).Message.ShouldContain("IConvention");
      Should.NotThrow(() => conventions.Register(typeof(DefaultTypeConvention)));
   }

   [Fact]
   public void TypeBuilderRejectsNullsAndUnknownMembers() {
      var builder = new EngineConfigurationTypeBuilder<SimpleUser>();
      IEngineConfigurationTypeBuilder untyped = builder;

      Should.Throw<ArgumentNullException>(() => new EngineConfigurationTypeBuilder(null!));
      Should.Throw<ArgumentNullException>(() => builder.Setup<string>(null!));
      Should.Throw<ArgumentNullException>(() => builder.Invoke(null!));
      Should.Throw<ArgumentNullException>(() => builder.ConstructWith<SimpleUserFactory>(null!));
      Should.Throw<ArgumentNullException>(() => untyped.ConstructWith(null!));
      Should.Throw<ArgumentException>(() => untyped.SetupProperty(" "));
      Should.Throw<ArgumentException>(() => untyped.SetupProperty("Nope")).Message.ShouldContain("Nope");
      Should.Throw<ArgumentException>(() => untyped.SetupField("Nope")).Message.ShouldContain("Nope");
      Should.Throw<ArgumentNullException>(() => builder.SetupMethod("SetPassword", null!));
   }

   [Fact]
   public void MemberBuilderRejectsNullsAndNonSources() {
      var builder = new EngineConfigurationTypeBuilder<SimpleUser>();
      var member = builder.Setup(u => u.FirstName);
      IEngineConfigurationTypeMemberBuilder untyped = (EngineConfigurationTypeMemberBuilder) member;

      Should.Throw<ArgumentNullException>(() => member.Use<FirstNameSource>((object[]) null!));
      Should.Throw<ArgumentNullException>(() => member.Use<FirstNameSource>((Action<FirstNameSource>) null!));
      Should.Throw<ArgumentNullException>(() => member.Use<FirstNameSource>((IDataSourceFactory<string>) null!));
      Should.Throw<ArgumentNullException>(() => untyped.Use(null!));
      Should.Throw<ArgumentException>(() => untyped.Use(typeof(string))).Message.ShouldContain("IDataSource");
   }

   [Fact]
   public void SessionRejectsNullsAndNegativeCounts() {
      var session = Session();

      Should.Throw<ArgumentNullException>(() => session.Next<SimpleUserRole>(null!));
      Should.Throw<ArgumentNullException>(() => session.Collection<SimpleUserRole>(1, null!));
      Should.Throw<ArgumentOutOfRangeException>(() => session.List<SimpleUserRole>(-1));
      Should.Throw<ArgumentOutOfRangeException>(() => session.Collection<SimpleUserRole>(-1));
   }

   [Fact]
   public void GeneratorAndCollectionRejectNullExpressions() {
      var session = Session();
      var single = session.Single<SimpleUserRole>();
      var list = session.List<SimpleUserRole>(2);

      Should.Throw<ArgumentNullException>(() => single.Impose<string>(null!, "x"));
      Should.Throw<ArgumentNullException>(() => single.Impose<string>(null!, r => "x"));
      Should.Throw<ArgumentNullException>(() => single.Source<string>(null!, new FirstNameSource()));
      Should.Throw<ArgumentNullException>(() => single.Source(r => r.Name, (IDataSource) null!));
      Should.Throw<ArgumentNullException>(() => single.Invoke(null!));
      Should.Throw<ArgumentNullException>(() => list.Impose<string>(null!, "x"));
      Should.Throw<ArgumentNullException>(() => list.Source<string>(null!, new FirstNameSource()));
      Should.Throw<ArgumentNullException>(() => list.Invoke(null!));
      Should.Throw<ArgumentOutOfRangeException>(() => list.First(-1));
      Should.Throw<ArgumentOutOfRangeException>(() => list.Random(-1));
      Should.Throw<ArgumentOutOfRangeException>(() => list.First(1).Next(-1));
      Should.Throw<ArgumentNullException>(() => list.First(1).Impose<string>(null!, "x"));
   }

   [Fact]
   public void DataSourceBaseRejectsNullEvaluator()
      => Should.Throw<ArgumentNullException>(() => new FirstNameSource().SetRandomNullEvaluator(null!));

   [Fact]
   public void FuncSourceWithContextLambdaRefusesToRunWithoutContext() {
      var source = new FuncSource<string>(ctx => ctx.Single<SimpleUserRole>().Get().Name);

      Should.Throw<InvalidOperationException>(() => source.Next(null)).Message.ShouldContain("session");
      new FuncSource<string>(() => "ok").Next(null).ShouldBe("ok");
   }

   [Fact]
   public void ErrorsInsideTheEngineCarryTheMemberAndTypeName() {
      var member = new EngineTypePropertyMember(typeof(SimpleUserRole).GetProperty(nameof(SimpleUserRole.Name))!);
      var node = new TypePropertyGenerationContextNode(new TypeGenerationContextNode(null, null), member);

      Should.Throw<InvalidOperationException>(() => node.Target).Message.ShouldContain("constructed");
      Should.Throw<InvalidOperationException>(() => new TypePropertyConventionContext(null!, new EngineConfigurationTypeMember(new EngineTypeFieldMember(typeof(SimpleFieldClass).GetField("SomeField")!))).Member)
         .Message.ShouldContain("SomeField");
   }

   private sealed class SimpleUserFactory : IDataSource<SimpleUser> {
      public object InternalNext(IGenerationContext? context) => throw new NotSupportedException();
   }
}
