using System.Linq.Expressions;
using holonsoft.AutoPoco.Actions;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Engine;

public class ObjectGenerator<T>(IGenerationContext session, IObjectBuilder type) : IObjectGenerator<T> {
   private readonly List<IObjectAction> _overrides = new();

   public T Get() {
      // Create the object     
      var createdObject = type.CreateObject(session);

      // And overrides
      var typeContext =
        new GenerationContext(session.Builders, new TypeGenerationContextNode(session.Node, createdObject));
      foreach (var action in _overrides)
         action.Enact(typeContext, createdObject);

      // And return the created object
      return (T) createdObject;
   }

   public IObjectGenerator<T> Impose<TMember>(Expression<Func<T, TMember>> propertyExpr, TMember value) {
      var member = ReflectionHelper.GetMember(propertyExpr);
      if (member.IsField)
         AddAction(new ObjectFieldSetFromValueAction((EngineTypeFieldMember) member, value!));
      else if (member.IsProperty)
         AddAction(new ObjectPropertySetFromValueAction((EngineTypePropertyMember) member, value!));

      return this;
   }

   public IObjectGenerator<T> Source<TMember>(Expression<Func<T, TMember>> propertyExpr, IDataSource dataSource) {
      var member = ReflectionHelper.GetMember(propertyExpr);
      if (member.IsField)
         AddAction(new ObjectFieldSetFromSourceAction((EngineTypeFieldMember) member, dataSource));
      else if (member.IsProperty)
         AddAction(new ObjectPropertySetFromSourceAction((EngineTypePropertyMember) member, dataSource));

      return this;
   }

   public IObjectGenerator<T> Invoke(Expression<Action<T>> methodExpr) {
      var invoker = new ObjectMethodInvokeActionAction<T>(methodExpr.Compile());
      _overrides.Add(invoker);
      return this;
   }

   public IObjectGenerator<T> Invoke<TMember>(Expression<Func<T, TMember>> methodExpr) {
      var invoker = new ObjectMethodInvokeFuncAction<T, TMember>(methodExpr.Compile());
      _overrides.Add(invoker);
      return this;
   }

   public void AddAction(IObjectAction action)
      => _overrides.Add(action);
}