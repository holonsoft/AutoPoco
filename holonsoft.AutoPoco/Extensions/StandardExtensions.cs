using System.Collections;
using System.Reflection;
using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Extensions;

public static class StandardExtensions {
   extension<TPoco, TMember>(IEngineConfigurationTypeMemberBuilder<TPoco, TMember> memberConfig) {
      /// <summary>
      ///   Sets the value of a member directly
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> Value(TMember value)
         => memberConfig.Use<ValueSource<TMember>>(value!);

      /// <summary>
      ///   Takes the value from the parent object that is currently being generated
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> FromParent()
         => memberConfig.Use<ParentSource<TMember>>();

      /// <summary>
      ///   Computes the member value with a lambda. The lambda runs once per generated object and decides
      ///   about the value on its own, including null. Handy for counters, formatted strings or anything
      ///   that is not worth a dedicated data source class.
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> From(Func<TMember> factory)
         => memberConfig.Use<FuncSource<TMember>>(factory);

      /// <summary>
      ///   Computes the member value with a lambda that receives the generation context, e.g. to build
      ///   related objects via <c>context.Single&lt;TOther&gt;()</c> or to walk up the parent chain.
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> From(Func<IGenerationContext?, TMember> factory)
         => memberConfig.Use<FuncSource<TMember>>(factory);
   }

   extension<TPoco, TCollection>(IEngineConfigurationTypeMemberBuilder<TPoco, TCollection> memberConfig)
      where TCollection : IEnumerable {
      /// <summary>
      ///   Fills a generic collection member with between min and max automatically generated items
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> Collection(int min, int max) {
         var collectionType = typeof(TCollection);

         // We need to find the base collection type (we only support generic collections of X for the moment)
         var collectionContentType = GetCollectionContentType(collectionType)
            ?? throw new ArgumentException(
               $"Unable to find collection type, only collections of type IEnumerable<T> are supported, wrong is '{collectionType.Name}'");

         // So this will give us AutoSource<Y>
         var autoSourceType = typeof(AutoSource<>).MakeGenericType(collectionContentType);

         // And this will give us FlexibleEnumerableSource<AutoSource<Y>, X<Y>, Y>
         var enumerableSourceType = typeof(FlexibleEnumerableSource<,,>).MakeGenericType(
           autoSourceType, collectionType, collectionContentType
         );

         // Use<TSource>(params object[]) has to be closed over a type that is only known at runtime, hence reflection
         var method = memberConfig.GetType().GetMethod("Use",
           BindingFlags.Public |
           BindingFlags.Instance, null, [typeof(object[])], null);

         var genericMethod = method?.MakeGenericMethod(enumerableSourceType);
         return (IEngineConfigurationTypeBuilder<TPoco>) genericMethod?.Invoke(memberConfig,
            [new object[] { min, max }])!;
      }
   }

   private static Type? GetCollectionContentType(Type? collectionType) => EnumerableExtensions.AncestorsAndSelf(collectionType)
         .FirstOrDefault(type => type.IsGenericType && type.GetGenericArguments().Length == 1)
         ?.GetGenericArguments()[0];
}
