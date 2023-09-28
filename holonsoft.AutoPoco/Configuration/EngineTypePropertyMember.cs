using System.Globalization;
using System.Reflection;

namespace holonsoft.AutoPoco.Configuration;

public class EngineTypePropertyMember(PropertyInfo propertyInfo) : EngineTypeMember {
   public override string Name => PropertyInfo.Name;

   public override bool IsMethod => false;

   public override bool IsField => false;

   public override bool IsProperty => true;

   public PropertyInfo PropertyInfo { get; } = propertyInfo ?? throw new ArgumentNullException($"{nameof(propertyInfo)}");

   public override bool Equals(object? obj) {
      var otherMember = obj as EngineTypePropertyMember;
      if (otherMember == null) return false;

      var propertyTwo = otherMember.PropertyInfo;

      if (string.Compare(PropertyInfo.Name, propertyTwo.Name, false, CultureInfo.InvariantCulture) != 0)
         return false;

      return PropertyInfo.PropertyType == propertyTwo.PropertyType;
   }

   public override int GetHashCode() => PropertyInfo.GetHashCode();
}