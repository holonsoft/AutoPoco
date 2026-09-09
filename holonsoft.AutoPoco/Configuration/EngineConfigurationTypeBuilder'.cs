using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeBuilder<TPoco> : EngineConfigurationTypeBuilder,
  IEngineConfigurationTypeBuilder<TPoco> {
   public EngineConfigurationTypeBuilder()
      : base(typeof(TPoco)) { }

   public IEngineConfigurationTypeMemberBuilder<TPoco, TMember> Setup<TMember>(
     Expression<Func<TPoco, TMember>> expression) {
      ArgumentNullException.ThrowIfNull(expression);

      // Get the member this set up is for
      var member = ReflectionHelper.GetMember(expression);

      // Create the configuration builder
      var configuration = new EngineConfigurationTypeMemberBuilder<TPoco, TMember>(member, this);

      // Store it in the local list
      RegisterTypeMemberProvider(configuration);

      // And return it
      return configuration;
   }

   public IEngineConfigurationTypeBuilder<TPoco> ConstructWith<TSource>() where TSource : IDataSource<TPoco> {
      ConstructWith(typeof(TSource));
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> ConstructWith<TSource>(params object[] args)
     where TSource : IDataSource<TPoco> {
      ArgumentNullException.ThrowIfNull(args);
      ConstructWith(typeof(TSource), args);
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> Invoke(Expression<Action<TPoco>> action) {
      ArgumentNullException.ThrowIfNull(action);
      var context = GetMethodArgs(action);
      var name = ReflectionHelper.GetMethodName(action);
      SetupMethod(name, context);
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> Invoke<TReturn>(Expression<Func<TPoco, TReturn>> func) {
      ArgumentNullException.ThrowIfNull(func);
      var context = GetMethodArgs(func);
      var name = ReflectionHelper.GetMethodName(func);
      SetupMethod(name, context);
      return this;
   }

   private static MethodInvocationContext GetMethodArgs(Expression<Action<TPoco>> action) {
      if (action.Body is not MethodCallExpression methodExpression)
         throw new ArgumentException(@"Method expression expected, and not passed in", nameof(action));
      return GetMethodArgs(methodExpression);
   }

   private static MethodInvocationContext GetMethodArgs<TReturn>(Expression<Func<TPoco, TReturn>> function) {
      if (function.Body is not MethodCallExpression methodExpression)
         throw new ArgumentException(@"Method expression expected, and not passed in", nameof(function));
      return GetMethodArgs(methodExpression);
   }

   /// <summary>
   ///   Turns every argument of the invoked method into a data source: <c>Use.Source</c> and <c>Use.From</c>
   ///   markers become the source they stand for, constants and captured variables become fixed values.
   ///   Other method calls are rejected, they would run at configuration time instead of per object.
   /// </summary>
   private static MethodInvocationContext GetMethodArgs(MethodCallExpression methodExpression) {
      var context = new MethodInvocationContext();
      foreach (var arg in methodExpression.Arguments)
         switch (arg) {
            case MethodCallExpression { Method.DeclaringType: var declaringType } call when declaringType == typeof(Use):
               AddMarkerArgument(context, call);
               break;
            case MethodCallExpression:
               throw new ArgumentException(
                  $"Unsupported argument in method invocation '{methodExpression.Method.Name}': a method call is only allowed as Use.Source<...>() or Use.From(...) marker.",
                  nameof(methodExpression));
            case ConstantExpression constant:
               context.AddArgumentValue(constant.Value);
               break;
            case MemberExpression or UnaryExpression { NodeType: ExpressionType.Convert }:
               // a captured variable, a field of a closure or a boxed value: evaluate it once, now
               context.AddArgumentValue(Expression.Lambda(arg).Compile().DynamicInvoke());
               break;
            default:
               throw new ArgumentException(
                  $"Unsupported argument in method invocation '{methodExpression.Method.Name}': {arg.NodeType} expressions are not supported, use a constant, a variable, Use.Source<...>() or Use.From(...).",
                  nameof(methodExpression));
         }

      return context;
   }

   private static void AddMarkerArgument(MethodInvocationContext context, MethodCallExpression marker) {
      switch (marker.Method.Name) {
         case nameof(Use.Source):
            context.AddArgumentSource(ExtractDataSourceType(marker), ExtractDataSourceParameters(marker));
            break;
         case nameof(Use.From):
            var valueType = marker.Method.GetGenericArguments()[0];
            var lambda = Expression.Lambda(marker.Arguments[0]).Compile().DynamicInvoke()
                         ?? throw new ArgumentException("Use.From needs a lambda.", nameof(marker));
            context.AddArgumentSource(typeof(FuncSource<>).MakeGenericType(valueType), lambda);
            break;
         default:
            throw new ArgumentException($"Unknown Use marker '{marker.Method.Name}'.", nameof(marker));
      }
   }

   private static Type ExtractDataSourceType(MethodCallExpression paramCall) {
      if (!paramCall.Method.IsGenericMethod)
         throw new ArgumentException(@"Method expression is not generic and types cannot be resolved", nameof(paramCall));

      var sourceType = paramCall.Method.GetGenericArguments().Skip(1).FirstOrDefault()
         ?? throw new ArgumentException(@"Method expression uses un-recognized generic method and types cannot be resolved");

      return sourceType;
   }

   private static object[] ExtractDataSourceParameters(MethodCallExpression paramCall) {
      if (paramCall.Arguments.Count == 0)
         return Array.Empty<object>();

      var args = new List<object>();

      if (paramCall.Arguments.Count > 1)
         throw new ArgumentException(@"Method expression uses unrecognized method and types cannot be resolved");
      if (paramCall.Arguments[0].NodeType != ExpressionType.NewArrayInit)
         throw new ArgumentException(@"Method expression uses unrecognized method and types cannot be resolved");

      // Each item in this array is an argument, but wrapped as a unary expression cos that's how it works
      var expr = paramCall.Arguments[0] as NewArrayExpression;
      foreach (var argumentExpression in expr!.Expressions.Cast<UnaryExpression>()) {
         if (argumentExpression.Operand is not ConstantExpression constantValue)
            throw new ArgumentException(@"Method expression uses unrecognized method and types cannot be resolved");

         args.Add(constantValue.Value
                  ?? throw new ArgumentException("Constructor arguments of a Use.Source marker must not be null.", nameof(paramCall)));
      }

      return args.ToArray();
   }
}
