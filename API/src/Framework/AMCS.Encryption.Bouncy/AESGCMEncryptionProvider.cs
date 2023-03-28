using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Encryption.BouncyCastle
{
  public class AESGCMEncryptionProvider : IEncryptionProvider
  {
    public static readonly IEncryptionProviderFactory Factory = new AESGCMEncryptionProviderFactory();

    private readonly string key;

    public int? Id => 2;

    private AESGCMEncryptionProvider(string key)
    {
      this.key = key;
    }

    public byte[] Encrypt(byte[] data)
    {
      return AESGCM.SimpleEncryptWithPassword(data, key);
    }

    public byte[] Decrypt(byte[] data)
    {
      return AESGCM.SimpleDecryptWithPassword(data, key);
    }

    private class AESGCMEncryptionProviderFactory : IEncryptionProviderFactory
    {
      public IEncryptionProvider Create(string key)
      {
        return new AESGCMEncryptionProvider(key);
      }
    }
  }
}
