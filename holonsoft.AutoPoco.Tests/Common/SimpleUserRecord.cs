namespace holonsoft.AutoPoco.Tests.Common;

public record SimpleUserRecord(string FirstName, string LastName, string EmailAddress, SimpleUserRoleRecord Role) {
   private string? _hiddenPassword;

   public string RevealedPassword
      => string.IsNullOrWhiteSpace(_hiddenPassword)
         ? "not set"
         : _hiddenPassword;

   public SimpleUserRecord()
      : this("", "", "", new SimpleUserRoleRecord("")) { }

   public void SetPassword(string password)
      => _hiddenPassword = password;
}