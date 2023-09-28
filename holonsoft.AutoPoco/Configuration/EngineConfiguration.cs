using holonsoft.AutoPoco.Configuration.Interfaces;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfiguration : IEngineConfiguration {
   private readonly List<EngineConfigurationType> _registeredTypes = new();

   public IEnumerable<IEngineConfigurationType> GetRegisteredTypes() 
      => _registeredTypes.ConvertAll(x => (IEngineConfigurationType) x);

   public void RegisterType(Type t) {
      if (_registeredTypes.Find(x => x.RegisteredType == t) != null)
         throw new ArgumentException($"Type has already been registered: '{nameof(t)}'");
      _registeredTypes.Add(new EngineConfigurationType(t));
   }

   public IEngineConfigurationType? GetRegisteredType(Type t) 
      => _registeredTypes.Find(x => x.RegisteredType == t);
}