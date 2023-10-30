namespace holonsoft.AutoPoco.Tests.Common;

public class SimpleUser {
   public required string FirstName { get; set; }

   public required string LastName { get; set; }

   public required string EmailAddress { get; set; }

   public required SimpleUserRole Role { get; set; }

   public DateOnly Birthday { get; set; }

   public Int128 Id { get; set; }

   public Int128 ExternalId { get; set; }

   public TimeSpan TimeSpan { get; set; }

   public string? City { get; set; }

   private string? _hiddenPassword;

   public string RevealedPassword
      => string.IsNullOrWhiteSpace(_hiddenPassword)
         ? "not set"
         : _hiddenPassword;

   public void SetPassword(string password)
      => _hiddenPassword = password;

   public required IList<SimpleUserProperty> Properties { get; set; }
}
