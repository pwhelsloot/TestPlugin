using System;
using System.Security.Cryptography;
using System.Text;

namespace AMCS.Data.Support.Security
{
  /// <summary>
  /// A standard implementation of an IStringEncryptor
  /// </summary>
  public class StringEncryptor : IStringEncryptor
  {
    #region Constants

    /// <summary>
    /// Encryption key.
    /// N.B. This isn't particularly secure and is open to easy discovery through decompilation and disassembly
    /// </summary>
    private const string Key = "&Sj2h8m2(sd?5aw";

    #endregion Constants

    private readonly Encoding encoding;

    /// <summary>
    /// This is the default encryptor. If you are unsure which one you need to use, it is probably this one.
    /// </summary>
    public static StringEncryptor DefaultEncryptor = new StringEncryptor(Encoding.ASCII);

    /// <summary>
    /// This should only be used if dealing with special characters.
    /// An example of such a character would be something unique to the Norwegian alphabet etc.
    /// </summary>
    public static StringEncryptor UnicodeEncryptor = new StringEncryptor(Encoding.Unicode);

    #region Methods

    private StringEncryptor(Encoding encoding)
    {
      this.encoding = encoding;
    }

    /// <summary>
    /// Decrypts the specified encrypted string.
    /// </summary>
    /// <param name="encryptedString">The encrypted string.</param>
    /// <returns>Decrypted string.</returns>
    public string Decrypt(string encryptedString)
    {
      if (string.IsNullOrEmpty(encryptedString))
      {
        return string.Empty;
      }

      byte[] inputBytes = null;
      if ((encryptedString.Length % 4) != 0)
        // has to be divisible by 4 if its encrypted
        throw new Exception("Encrypted string is not of legal length.");

      inputBytes = Convert.FromBase64String(encryptedString);

      string decrypted = null;
      byte[] pwdhash = null;
      MD5CryptoServiceProvider hashmd5;

      // generate an MD5 hash from the password.
      // a hash is a one way encryption meaning once you generate
      // the hash, you cant derive the password back from it.
      hashmd5 = new MD5CryptoServiceProvider();
      pwdhash = hashmd5.ComputeHash(encoding.GetBytes(Key));
      hashmd5 = null;

      // Create a new TripleDES service provider
      TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
      tdesProvider.Key = pwdhash;
      tdesProvider.Mode = CipherMode.ECB;

      decrypted = encoding.GetString(
          tdesProvider.CreateDecryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));

      return decrypted;
    }

    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    /// <param name="plainText">The plain text.</param>
    /// <returns>Encrypted string.</returns>
    public string Encrypt(string plainText)
    {
      if (string.IsNullOrEmpty(plainText))
      {
        return string.Empty;
      }

      string encrypted = null;

      byte[] inputBytes = encoding.GetBytes(plainText);
      byte[] pwdhash = null;
      MD5CryptoServiceProvider hashmd5;

      // generate an MD5 hash from the password.
      // a hash is a one way encryption meaning once you generate
      // the hash, you cant derive the password back from it.
      hashmd5 = new MD5CryptoServiceProvider();
      pwdhash = hashmd5.ComputeHash(encoding.GetBytes(Key));
      hashmd5 = null;

      // Create a new TripleDES service provider
      TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
      tdesProvider.Key = pwdhash;
      tdesProvider.Mode = CipherMode.ECB;

      encrypted = Convert.ToBase64String(
          tdesProvider.CreateEncryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));

      return encrypted;
    }

    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    /// <param name="plainText">The plain text.</param>
    /// <returns>Hashed string.</returns>
    public string Hash(string plainText, string donorSaltComponent, string additionalDonorSaltComponent)
    {
      string salt = PasswordHasher.CreateSalt(donorSaltComponent, additionalDonorSaltComponent);

      string PasswordHash = PasswordHasher.HashPasswordAsBase64(plainText, salt);

      return PasswordHash;
    }

    #endregion Methods
  }
}
