using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Encryption
{
  public static class EncryptionServiceExtensions
  {
    public static string Encrypt(this IEncryptionService self, string data)
    {
      return Encrypt(self, data, Encoding.UTF8);
    }

    public static string Encrypt(this IEncryptionService self, string data, Encoding encoding)
    {
      var decrypted = encoding.GetBytes(data);

      var encrypted = self.Encrypt(decrypted);

      return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(this IEncryptionService self, string data)
    {
      return Decrypt(self, data, Encoding.UTF8);
    }

    public static string Decrypt(this IEncryptionService self, string data, Encoding encoding)
    {
      var encrypted = Convert.FromBase64String(data);

      var decrypted = self.Decrypt(encrypted);

      return encoding.GetString(decrypted);
    }
  }
}
