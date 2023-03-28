using AMCS.Data.Support.Security;

namespace AMCS.Data.Server.Util
{
  partial class PasswordHashing
  {
    private class LegacyAlgorithm : IPasswordHashingAlgorithm
    {
      public bool IsMatch(string hashedPassword) => true;

      public string Hash(string userName, string password)
      {
        string salt = PasswordHasher.CreateSalt(userName, password);
        return PasswordHasher.HashPasswordAsBase64(password, salt);
      }

      public bool Verify(string userName, string password, string hashedPassword)
      {
        var salt = PasswordHasher.CreateSalt(userName, password);
        return PasswordHasher.ValidatePassword(password, salt, hashedPassword);
      }
    }
  }
}
