using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationType(Type t) : IEngineConfigurationType {
   private IEngineConfigurationDataSource? _factory;
   private readonly List<EngineConfigurationTypeMember> _registeredMembers = new();

   public Type RegisteredType { get; } = t;

   public void RegisterMember(EngineTypeMember member) {
      if (_registeredMembers.Find(x => x.Member == member) != null)
         throw new ArgumentException(@"Member has already been registered", nameof(member));
      _registeredMembers.Add(new EngineConfigurationTypeMember(member));
   }

   public IEngineConfigurationTypeMember GetRegisteredMember(EngineTypeMember member)
      => _registeredMembers.Find(x => x.Member == member)!;
            

   public IEnumerable<IEngineConfigurationTypeMember> GetRegisteredMembers() 
      => _registeredMembers.ConvertAll(x => (IEngineConfigurationTypeMember) x);

   public void SetFactory(IEngineConfigurationDataSource factory) 
      => _factory = factory;

   public IEngineConfigurationDataSource? GetFactory() 
      => _factory;
}