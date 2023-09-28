using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeBuilder(Type type) : IEngineConfigurationTypeProvider, IEngineConfigurationTypeBuilder {
   private DataSourceFactory? _factory;
   private readonly List<IEngineConfigurationTypeMemberProvider> _members = new();

   IEngineConfigurationTypeMemberBuilder IEngineConfigurationTypeBuilder.SetupProperty(string propertyName) {
      MemberInfo info = type.GetProperty(propertyName) ?? throw new ArgumentException($"Property does not exist: '{propertyName}'");
      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      return memberBuilder;
   }

   IEngineConfigurationTypeMemberBuilder IEngineConfigurationTypeBuilder.SetupField(string fieldName) {
      var info = type.GetField(fieldName) ?? throw new ArgumentException($"Field does not exist: '{fieldName}'");
      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      return memberBuilder;
   }

   public IEngineConfigurationTypeBuilder SetupMethod(string methodName, MethodInvocationContext context) {
      var factories = context.GetArguments().ToArray();
      var info = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == methodName
                                         && x.GetParameters().Length == factories.Length)
                 ?? throw new ArgumentException($"Method does not exist: '{methodName}'");

      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      memberBuilder.SetDataSources(factories);

      return this;
   }

   public IEngineConfigurationTypeBuilder ConstructWith(Type type) {
      _factory = new DataSourceFactory(type);
      return this;
   }

   public IEngineConfigurationTypeBuilder ConstructWith(Type type, params object[] args) {
      _factory = new DataSourceFactory(type);
      _factory.SetParams(args);
      return this;
   }

   Type IEngineConfigurationTypeProvider.GetConfigurationType()
      => type;

   IEnumerable<IEngineConfigurationTypeMemberProvider> IEngineConfigurationTypeProvider.GetConfigurationMembers()
      => _members;

   IEngineConfigurationDataSource IEngineConfigurationTypeProvider.GetFactory()
      => _factory!;

   public IEngineConfigurationTypeBuilder SetupMethod(string methodName)
      => SetupMethod(methodName, new MethodInvocationContext());

   public void RegisterTypeMemberProvider(IEngineConfigurationTypeMemberProvider memberProvider)
      => _members.Add(memberProvider);
}