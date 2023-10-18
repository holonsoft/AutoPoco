namespace holonsoft.AutoPoco.Tests.Common;

public record SimpleUserRecord(string FirstName, string LastName, string EmailAddress, DateOnly Birthday, Int128 Id, Int128 ExternalId, 
                               string City, SimpleUserRoleRecord Role) {
   private string? _hiddenPassword;

   public string RevealedPassword
      => string.IsNullOrWhiteSpace(_hiddenPassword)
         ? "not set"
         : _hiddenPassword;

   public SimpleUserRecord()
      : this("", "", "", DateOnly.MinValue, 0, 0,  "no-city", new SimpleUserRoleRecord("")) { }

   public void SetPassword(string password)
      => _hiddenPassword = password;
}