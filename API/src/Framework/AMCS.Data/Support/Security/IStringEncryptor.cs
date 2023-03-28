namespace AMCS.Data.Support.Security
{
  /// <summary>
  /// Represents an encryptor that can encrypt and decrypt, to and from strings.
  /// </summary>
  public interface IStringEncryptor
  {
    string Encrypt(string plainText);
    string Hash(string plainText, string donorSaltComponent, string additionalDonorSaltComponent);
    string Decrypt(string cypherText);
  }
}
