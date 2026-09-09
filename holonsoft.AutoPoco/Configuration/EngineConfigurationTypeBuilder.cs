using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeBuilder : IEngineConfigurationTypeProvider, IEngineConfigurationTypeBuilder {
   private readonly Type _type;
   private AutoPocoDataSourceFactory? _factory;
   private readonly List<IEngineConfigurationTypeMemberProvider> _members = new();

   public EngineConfigurationTypeBuilder(Type type) {
      ArgumentNullException.ThrowIfNull(type);
      _type = type;
   }

   IEngineConfigurationTypeMemberBuilder IEngineConfigurationTypeBuilder.SetupProperty(string propertyName) {
      ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
      MemberInfo info = _type.GetProperty(propertyName) ?? throw new ArgumentException($"Property does not exist: '{propertyName}'", nameof(propertyName));
      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      return memberBuilder;
   }

   IEngineConfigurationTypeMemberBuilder IEngineConfigurationTypeBuilder.SetupField(string fieldName) {
      ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
      var info = _type.GetField(fieldName) ?? throw new ArgumentException($"Field does not exist: '{fieldName}'", nameof(fieldName));
      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      return memberBuilder;
   }

   public IEngineConfigurationTypeBuilder SetupMethod(string methodName, MethodInvocationContext context) {
      ArgumentException.ThrowIfNullOrWhiteSpace(methodName);
      ArgumentNullException.ThrowIfNull(context);

      var factories = context.GetArguments().ToArray();
      var info = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == methodName
                                         && x.GetParameters().Length == factories.Length)
                 ?? throw new ArgumentException($"Method does not exist: '{methodName}' with {factories.Length} parameter(s)", nameof(methodName));

      var memberBuilder = new EngineConfigurationTypeMemberBuilder(ReflectionHelper.GetMember(info), this);
      _members.Add(memberBuilder);
      memberBuilder.SetDataSources(factories);

      return this;
   }

   public IEngineConfigurationTypeBuilder ConstructWith(Type type) {
      ArgumentNullException.ThrowIfNull(type);
      _factory = new AutoPocoDataSourceFactory(type);
      return this;
   }

   public IEngineConfigurationTypeBuilder ConstructWith(Type type, params object[] args) {
      ArgumentNullException.ThrowIfNull(type);
      ArgumentNullException.ThrowIfNull(args);
      _factory = new AutoPocoDataSourceFactory(type);
      _factory.SetParams(args);
      return this;
   }

   Type IEngineConfigurationTypeProvider.GetConfigurationType()
      => _type;

   IEnumerable<IEngineConfigurationTypeMemberProvider> IEngineConfigurationTypeProvider.GetConfigurationMembers()
      => _members;

   IEngineConfigurationDataSource IEngineConfigurationTypeProvider.GetFactory()
      => _factory!;

   public IEngineConfigurationTypeBuilder SetupMethod(string methodName)
      => SetupMethod(methodName, new MethodInvocationContext());

   public void RegisterTypeMemberProvider(IEngineConfigurationTypeMemberProvider memberProvider) {
      ArgumentNullException.ThrowIfNull(memberProvider);
      _members.Add(memberProvider);
   }
}
