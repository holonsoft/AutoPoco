namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleDerivedClass : SimpleBaseClass, ISimpleInterface {
   public required string Name { get; set; }

   public override required string BaseVirtualProperty { get; set; }
}