using System.Diagnostics.CodeAnalysis;

namespace holonsoft.AutoPoco.Tests.Common;

/// <summary>
///   Every flavor of nullability the engine has to recognize.
/// </summary>
public class NullableMembersClass {
   public string NonNullableText { get; set; } = "";

   public string? NullableText { get; set; }

   public int NonNullableNumber { get; set; }

   public int? NullableNumber { get; set; }

   public DateTime? NullableDate { get; set; }

   public SimpleUserRole? NullableReference { get; set; }

   [AllowNull]
   public string AllowNullText { get; set; } = "";

   [DisallowNull]
   public string? DisallowNullText { get; set; }

   public string? GetOnlyNullable => null;

   public string GetOnlyNonNullable => "";

   public string? NullableField;

   public string NonNullableField = "";

   public int? NullableNumberField;

   public void Method(string? nullableParameter, string nonNullableParameter, int? nullableNumber, int number) { }
}
