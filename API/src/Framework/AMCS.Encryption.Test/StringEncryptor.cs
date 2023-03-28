using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Encryption.Test
{
  [Obsolete("Use EncryptionService instead.")]
  public class StringEncryptor
  {
    public const string Key = "&Sj2h8m2(sd?5aw";

    public static string Decrypt(string encryptedString)
    {
      if (string.IsNullOrEmpty(encryptedString))
        return string.Empty;
      if (encryptedString.Length % 4 != 0)
        throw new Exception("Encrypted string is not of legal length.");
      byte[] inputBuffer = Convert.FromBase64String(encryptedString);
      byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Key));
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      cryptoServiceProvider.Key = hash;
      cryptoServiceProvider.Mode = CipherMode.ECB;
      return Encoding.ASCII.GetString(cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
    }

    public static string Encrypt(string plainText)
    {
      if (string.IsNullOrEmpty(plainText))
        return string.Empty;
      byte[] bytes = Encoding.ASCII.GetBytes(plainText);
      byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Key));
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      cryptoServiceProvider.Key = hash;
      cryptoServiceProvider.Mode = CipherMode.ECB;
      return Convert.ToBase64String(cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
    }
  }
}
