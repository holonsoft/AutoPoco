using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

public class ObjectBuilder : IObjectBuilder {
   private readonly List<IObjectAction> _actions = new();
   private readonly IDataSource? _factory;

   /// <summary>
   ///   Creates this object builder
   /// </summary>
   /// <param name="type">the registered type this builder creates</param>
   /// <param name="nullableAnnotations">optional, when enabled the sources of nullable members are wrapped in a <see cref="NullableMemberDataSource" /></param>
   public ObjectBuilder(IEngineConfigurationType type, NullableAnnotationSettings? nullableAnnotations = null) {
      ArgumentNullException.ThrowIfNull(type);
      InnerType = type.RegisteredType;

      if (type.GetFactory() != null)
         _factory = type.GetFactory()?.Build() ?? throw new InvalidOperationException();

      // build every source once, in registration order; the factory may take some of the members over
      var entries = new List<(EngineTypeMember Member, List<IDataSource?> Sources)>();
      var memberSources = new List<MemberSource>();

      foreach (var registered in type.GetRegisteredMembers()) {
         var sources = registered.GetDataSources().Select(s => s.Build()).ToList();
         if (!registered.Member.IsMethod) {
            if (sources.Count == 0)
               continue;

            var source = NullableMemberDataSource.ForMember(sources[0] ?? throw new InvalidOperationException(), registered.Member, nullableAnnotations);
            sources[0] = source;
            memberSources.Add(new MemberSource(registered.Member, source));
         }

         entries.Add((registered.Member, sources));
      }

      var consumed = _factory is IMemberBoundFactory boundFactory
         ? boundFactory.BindMembers(memberSources)
         : [];

      foreach (var (member, sources) in entries) {
         if (member.IsMethod) {
            AddAction(new ObjectMethodInvokeFromSourceAction((EngineTypeMethodMember) member, sources!));
            continue;
         }

         if (consumed.Contains(member))
            continue;

         if (!ConstructorResolver.CanBeSet(member)) {
            if (_factory is IMemberBoundFactory)
               throw new InvalidOperationException(
                  $"Property '{member.Name}' of type '{InnerType.FullName}' has no setter and no public constructor of the type takes a parameter with the same name and type, so it cannot be populated.");

            // a custom factory is in charge of the object, nothing can be done for this member
            continue;
         }

         var source = sources[0] ?? throw new InvalidOperationException();
         if (member.IsField)
            AddAction(new ObjectFieldSetFromSourceAction((EngineTypeFieldMember) member, source));
         else if (member.IsProperty)
            AddAction(new ObjectPropertySetFromSourceAction((EngineTypePropertyMember) member, source));
      }
   }

   public Type InnerType { get; }

   public IEnumerable<IObjectAction> Actions => _actions;

   public void ClearActions() => _actions.Clear();

   public void AddAction(IObjectAction action) => _actions.Add(action);

   public void RemoveAction(IObjectAction action) => _actions.Remove(action);

   public object CreateObject(IGenerationContext context) {
      object? createdObject = null;

      if (_factory != null)
         createdObject = _factory.InternalNext(context);
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
