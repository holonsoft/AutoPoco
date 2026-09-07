#nullable disable

namespace holonsoft.AutoPoco.Tests.Common;

/// <summary>
///   Compiled without nullable annotations. The engine must treat these reference members as not nullable.
/// </summary>
public class ObliviousMembersClass {
   public string Text { get; set; }

   public int? Number { get; set; }

   public string Field;
}
