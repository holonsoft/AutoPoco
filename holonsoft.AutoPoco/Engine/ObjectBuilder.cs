using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public class ObjectBuilder : IObjectBuilder {
   private readonly List<IObjectAction> _actions = new();
   private readonly IDataSource? _factory;

   /// <summary>
   ///   Creates this object builder
   /// </summary>
   /// <param name="type"></param>
   public ObjectBuilder(IEngineConfigurationType type) {
      InnerType = type.RegisteredType;

      if (type.GetFactory() != null)
         _factory = type.GetFactory()?.Build() ?? throw new InvalidOperationException();

      type.GetRegisteredMembers()
        .ToList()
        .ForEach(x => {
           var sources = x.GetDataSources().Select(s => s.Build()).ToList();

           if (x.Member.IsField) {
              if (sources.Count == 0)
                 return;

              AddAction(new ObjectFieldSetFromSourceAction(
              (EngineTypeFieldMember) x.Member,
              sources.First() ?? throw new InvalidOperationException()));
           } else if (x.Member.IsProperty) {
              if (sources.Count == 0)
                 return;

              AddAction(new ObjectPropertySetFromSourceAction(
              (EngineTypePropertyMember) x.Member,
              sources.First() ?? throw new InvalidOperationException()));
           } else if (x.Member.IsMethod)
              AddAction(new ObjectMethodInvokeFromSourceAction(
              (EngineTypeMethodMember) x.Member,
              sources!
            ));
        });
   }

   public Type InnerType { get; }

   public IEnumerable<IObjectAction> Actions => _actions;

   public void ClearActions() => _actions.Clear();

   public void AddAction(IObjectAction action) => _actions.Add(action);

   public void RemoveAction(IObjectAction action) => _actions.Remove(action);

   public object CreateObject(IGenerationContext context) {
      object? createdObject = null;

      if (_factory != null)
         createdObject = _factory.Next(context);
      else
         createdObject = Activator.CreateInstance(InnerType);

      // Don't set it up if we've reached recursion limit
      if (context.Depth < context.Builders.RecursionLimit)
         EnactActionsOnObject(context, createdObject!);
      return createdObject!;
   }

   private void EnactActionsOnObject(IGenerationContext context, object createdObject) {
      var typeContext =
        new GenerationContext(context.Builders, new TypeGenerationContextNode(context.Node, createdObject));
      foreach (var action in _actions)
         action.Enact(typeContext, createdObject);
   }
}