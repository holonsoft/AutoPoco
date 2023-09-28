namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleMethodClass {
   public string? Value { get; set; }

   public string? OtherValue { get; set; }

   public bool ReturnSomethingCalled { get; set; }

   public void SetSomething(string value) => Value = value;

   public void SetSomething(string value, string otherValue) {
      Value = value;
      OtherValue = otherValue;
   }

   public string ReturnSomething() {
      ReturnSomethingCalled = true;
      return "";
   }
}