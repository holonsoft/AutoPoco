using System.Drawing;
using System.Globalization;
using System.Text;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

/// <summary>
///   Formats generated values as C# source, so a stable sequence test that fails after an intended
///   change prints the new expected array ready to paste.
/// </summary>
public static class CSharpLiteral {
   public static string Array<T>(IEnumerable<T> values) {
      var type = typeof(T);
      var elementType = Nullable.GetUnderlyingType(type) ?? type;
      var list = values.ToList();
      var nullable = type != elementType || (!type.IsValueType && list.Any(v => v is null));
      var typeName = TypeName(elementType) + (nullable ? "?" : "");
      return $"new {typeName}[] {{ {string.Join(", ", list.Select(v => Value(v)))} }}";
   }

   public static string Value(object? value)
      => value switch {
         null => "null",
         string s => Quote(s),
         bool b => b ? "true" : "false",
         int or long or short or byte or sbyte or ushort or uint or ulong => Convert.ToString(value, CultureInfo.InvariantCulture)!,
         Int128 or UInt128 => $"{TypeName(value.GetType())}.Parse(\"{value}\")",
         decimal m => m.ToString(CultureInfo.InvariantCulture) + "M",
         double d => d.ToString("R", CultureInfo.InvariantCulture) + "D",
         float f => f.ToString("R", CultureInfo.InvariantCulture) + "F",
         DateTime dt => $"new DateTime({dt.Ticks}, DateTimeKind.{dt.Kind})",
         DateOnly d => $"new DateOnly({d.Year}, {d.Month}, {d.Day})",
         TimeOnly t => $"new TimeOnly({t.Ticks})",
         TimeSpan ts => $"new TimeSpan({ts.Ticks})",
         Guid g => $"new Guid(\"{g}\")",
         Color c => $"Color.FromArgb({c.A}, {c.R}, {c.G}, {c.B})",
         Enum e => $"{e.GetType().Name}.{e}",
         _ => value.ToString() ?? "null"
      };

   private static string TypeName(Type type)
      => type switch {
         _ when type == typeof(int) => "int",
         _ when type == typeof(long) => "long",
         _ when type == typeof(string) => "string",
         _ when type == typeof(bool) => "bool",
         _ when type == typeof(decimal) => "decimal",
         _ when type == typeof(double) => "double",
         _ when type == typeof(float) => "float",
         _ => type.Name
      };

   private static string Quote(string value) {
      var builder = new StringBuilder("\"");
      foreach (var c in value)
         builder.Append(c switch {
            '"' => "\\\"",
            '\\' => "\\\\",
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            _ when c < ' ' => $"\\u{(int) c:x4}",
            _ => c.ToString()
         });

      return builder.Append('"').ToString();
   }
}
