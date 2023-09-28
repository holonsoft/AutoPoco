namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleBaseClass : ISimpleInterface {
   public required string BaseProperty { get; set; }

   public virtual required string BaseVirtualProperty { get; set; }

   public required string InterfaceValue { set; get; }

   public required string OtherInterfaceValue { get; set; }

   public void DoSomething() {
   }
}