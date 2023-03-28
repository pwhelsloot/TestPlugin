using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Encryption
{
  public class AESThenHMACEncryptionProvider : IEncryptionProvider
  {
    public static readonly IEncryptionProviderFactory Factory = new AESThenHMACEncryptionProviderFactory();

    private readonly string key;

    public int? Id => 1;

    private AESThenHMACEncryptionProvider(string key)
    {
      this.key = key;
    }

    public byte[] Encrypt(byte[] data)
    {
      return AESThenHMAC.SimpleEncryptWithPassword(data, key);
    }

    public byte[] Decrypt(byte[] data)
    {
      return AESThenHMAC.SimpleDecryptWithPassword(data, key);
    }

    private class AESThenHMACEncryptionProviderFactory : IEncryptionProviderFactory
    {
      public IEncryptionProvider Create(string key)
      {
        return new AESThenHMACEncryptionProvider(key);
      }
    }
  }
}
