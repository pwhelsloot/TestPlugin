using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Util
{
  partial class PasswordHashing
  {
    private class BCryptAlgorithm : IPasswordHashingAlgorithm
    {
      private const string Prefix = "{BC}";

      public bool IsMatch(string hashedPassword)
      {
        return hashedPassword.StartsWith(Prefix);
      }

      public string Hash(string userName, string password)
      {
        return Prefix + BCrypt.Net.BCrypt.HashPassword(password);
      }

      public bool Verify(string userName, string password, string hashedPassword)
      {
        if (!hashedPassword.StartsWith(Prefix))
          throw new ArgumentException("Hashed password doesn't start with prefix");

        return BCrypt.Net.BCrypt.Verify(password, hashedPassword.Substring(Prefix.Length));
      }
    }
  }
}
