namespace holonsoft.AutoPoco.Extensions;

public static class EnumerableExtensions {
   public static IEnumerable<Type> AncestorsAndSelf(Type? type) {
      while (type != null) {
         yield return type;
         type = type.BaseType;
      }
   }
}