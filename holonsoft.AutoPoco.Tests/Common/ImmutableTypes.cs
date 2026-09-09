namespace holonsoft.AutoPoco.Tests.Common;

/// <summary>
///   Positional record without a parameterless constructor.
/// </summary>
public record ImmutableUserRecord(string FirstName, string LastName, string? Nickname, DateOnly Birthday, ImmutableRoleRecord Role);

public record ImmutableRoleRecord(string Name);

/// <summary>
///   Class with get-only properties and two constructors, the shorter one defaults the currency.
/// </summary>
public class ImmutableMoney {
   public ImmutableMoney(decimal amount)
      : this(amount, "EUR") { }

   public ImmutableMoney(decimal amount, string currency) {
      Amount = amount;
      Currency = currency;
   }

   public decimal Amount { get; }
   public string Currency { get; }
}

/// <summary>
///   Self referencing record, the child is a constructor parameter.
/// </summary>
public record TreeNodeRecord(string Name, TreeNodeRecord? Child);

/// <summary>
///   Ordinary class with a parameterless and a parameterized constructor; the parameterized one leaves a trace.
/// </summary>
public class ClassWithBothCtors {
   public ClassWithBothCtors()
      => Name = "default";

   public ClassWithBothCtors(string name)
      => Name = name + "!";

   public string Name { get; set; }
}

/// <summary>
///   Get-only property whose name does not match the constructor parameter.
/// </summary>
public class ClassWithUnmatchedReadOnlyProperty(decimal amount) {
   public decimal Total { get; } = amount;
}

/// <summary>
///   Constructor parameter with a declared default and one without any counterpart.
/// </summary>
public class ClassWithDefaultedCtorParameter(string name, int retries = 3, ImmutableRoleRecord? role = null) {
   public string Name { get; } = name;
   public int Retries { get; } = retries;
   public ImmutableRoleRecord? Role { get; } = role;
}
