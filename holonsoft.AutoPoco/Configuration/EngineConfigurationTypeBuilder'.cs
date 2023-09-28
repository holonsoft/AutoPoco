using System.Linq.Expressions;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Configuration;

public class EngineConfigurationTypeBuilder<TPoco> : EngineConfigurationTypeBuilder,
  IEngineConfigurationTypeBuilder<TPoco> {
   public EngineConfigurationTypeBuilder()
      : base(typeof(TPoco)) { }

   public IEngineConfigurationTypeMemberBuilder<TPoco, TMember> Setup<TMember>(
     Expression<Func<TPoco, TMember>> expression) {
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
      ConstructWith(typeof(TSource), args);
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> Invoke(Expression<Action<TPoco>> action) {
      var context = GetMethodArgs(action);
      var name = ReflectionHelper.GetMethodName(action);
      SetupMethod(name, context);
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> Invoke<TReturn>(Expression<Func<TPoco, TReturn>> func) {
      var context = GetMethodArgs(func);
      var name = ReflectionHelper.GetMethodName(func);
      SetupMethod(name, context);
      return this;
   }

   public IEngineConfigurationTypeBuilder<TPoco> Ctor(Expression<Func<TPoco>> creationExpr) {
      var ctor = creationExpr.Body as NewExpression;
      // this.fa
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

   private static MethodInvocationContext GetMethodArgs(MethodCallExpression methodExpression) {
      var context = new MethodInvocationContext();
      foreach (var arg in methodExpression.Arguments)
         switch (arg.NodeType) {
            case ExpressionType.Call:

               var paramCall = arg as MethodCallExpression;

               // Extract the data source type
               var sourceType = ExtractDataSourceType(paramCall!);
               var factoryArgs = ExtractDataSourceParameters(paramCall!);

               context.AddArgumentSource(sourceType, factoryArgs);

               break;
            case ExpressionType.Constant:

               // Simply pop the constant into the list
               var paramConstant = arg as ConstantExpression;
               context.AddArgumentValue(paramConstant!);
               break;
            default:
               throw new ArgumentException(@"Unsupported argument used in method invocation list", nameof(methodExpression));
         }

      return context;
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

         args.Add(constantValue.Value ?? throw new InvalidOperationException());
      }

      return args.ToArray();
   }
}