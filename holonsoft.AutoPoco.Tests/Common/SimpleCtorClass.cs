namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleCtorClass {
   public string ReadOnlyProperty { get; }
   public string SecondaryProperty { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
   public SimpleCtorClass(string arg) => ReadOnlyProperty = arg;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

   public SimpleCtorClass(string argOne, string argTwo) {
      ReadOnlyProperty = argOne;
      SecondaryProperty = argTwo;
   }
}