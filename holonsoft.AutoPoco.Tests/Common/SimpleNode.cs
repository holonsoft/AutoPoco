namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleNode {
   public required SimpleNode Parent { get; set; }
   public required List<SimpleNode> Children { get; set; }
}