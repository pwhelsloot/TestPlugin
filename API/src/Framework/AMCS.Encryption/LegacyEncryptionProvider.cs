using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Encryption
{
  public class LegacyEncryptionProvider : IEncryptionProvider
  {
    public static readonly IEncryptionProviderFactory Factory = new LegacyEncryptionProviderFactory();

    private readonly byte[] key;

    // The legacy encryption provider does not have an ID. This encryption
    // provider is identified by the absence of an ID.
    public int? Id => null;

    public LegacyEncryptionProvider(byte[] key)
    {
      this.key = key;
    }

    private TripleDESCryptoServiceProvider CreateCryptoServiceProvider()
    {
      return new TripleDESCryptoServiceProvider { Key = key, Mode = CipherMode.ECB };
    }

    public byte[] Encrypt(byte[] data)
    {
      throw new NotSupportedException("This is no longer supported as it is a weak encryption algorithm");
    }

    public byte[] Decrypt(byte[] data)
    {
      using (var cryptoServiceProvider = CreateCryptoServiceProvider())
      using (var decryptor = cryptoServiceProvider.CreateDecryptor())
      {
        return decryptor.TransformFinalBlock(data, 0, data.Length);
      }
    }

    private class LegacyEncryptionProviderFactory : IEncryptionProviderFactory
    {
      public IEncryptionProvider Create(string key)
      {
        using (var md5 = new MD5CryptoServiceProvider())
        {
          return new LegacyEncryptionProvider(
            md5.ComputeHash(Encoding.UTF8.GetBytes(key))
          );
        }
      }
    }
  }
}
