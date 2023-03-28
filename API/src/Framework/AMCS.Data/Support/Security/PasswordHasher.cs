using System;
using System.Security.Cryptography;

namespace AMCS.Data.Support.Security
{
  public static class PasswordHasher
  {
    // once in production this can never change or stored databases will not be comparable
    /// <summary>
    /// The PBKDF2 iteration count
    /// </summary>
    private const int Iterations = 2000;
    /// <summary>
    /// The length of the hash to generate, in bytes.
    /// </summary>
    private const int HASH_BYTE_SIZE = 20;

    /// <summary>
    /// The length of the salt to generate, in bytes.
    /// </summary>
    public const int SALT_BYTE_SIZE = 24;
    /// <summary>
    /// Hashes the password as base64.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="salt">The salt.</param>
    /// <returns></returns>
    public static string HashPasswordAsBase64(string password, string salt)
    {
      Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(salt), PasswordHasher.Iterations);
      return Convert.ToBase64String(PasswordHasher.HashPassword(password, salt));
    }

    /// <summary>
    /// Computes the PBKDF2-SHA1 hash of a password.
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <param name="salt">The salt.</param>
    /// <returns></returns>
    public static byte[] HashPassword(string password, string salt)
    {
      Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(salt), PasswordHasher.Iterations);
      return hasher.GetBytes(PasswordHasher.HASH_BYTE_SIZE);
    }

    /// <summary>
    /// Generates a long random salt using a CSPRNG
    /// </summary>
    /// <returns></returns>
    public static string CreateSalt(string donorComponent, string additionalDonorComponent)
    {
      if (string.IsNullOrEmpty(donorComponent) || string.IsNullOrEmpty(additionalDonorComponent))
      {
        throw new ArgumentNullException("Salt components not provided");
      }

      string salt = donorComponent + additionalDonorComponent;

      while (salt.Length < 24)
      {
        salt = salt + donorComponent + additionalDonorComponent;
      }

      salt = salt.ToLower();
      return salt.Substring(0, 24);
    }

    /// <summary>
    /// Compares two byte arrays in length-constant time. This comparison
    /// method is used so that password hashes cannot be extracted from
    /// on-line systems using a timing attack and then attacked off-line.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>True if both byte arrays are equal. False otherwise.</returns>
    private static bool SlowEquals(byte[] a, byte[] b)
    {
      uint diff = (uint)a.Length ^ (uint)b.Length;
      for (int i = 0; i < a.Length && i < b.Length; i++)
        diff |= (uint)(a[i] ^ b[i]);
      return diff == 0;
    }

    /// <summary>
    /// Validates the password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="dbSalt">The database salt.</param>
    /// <param name="dbHash">The database hash.</param>
    /// <returns></returns>
    public static bool ValidatePassword(string password, string dbSalt, string dbHash)
    {
      byte[] hash = Convert.FromBase64String(dbHash);
      byte[] testHash = PasswordHasher.HashPassword(password, dbSalt);
      return SlowEquals(hash, testHash);
    }

  }
}
