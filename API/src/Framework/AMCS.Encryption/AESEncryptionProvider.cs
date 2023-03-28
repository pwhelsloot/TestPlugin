using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AMCS.Encryption
{
  public class AESEncryptionProvider : IEncryptionProvider
  {
    public static readonly IEncryptionProviderFactory Factory = new AESEncryptionProviderFactory();

    public const int BlockBitSize = 128;
    public const int KeyBitSize = 256;

    private readonly byte[] cryptKey;

    public int? Id => 3;

    private AESEncryptionProvider(byte[] cryptKey)
    {
      this.cryptKey = cryptKey;
    }

    public byte[] Encrypt(byte[] data)
    {
      using (var cipherStream = new MemoryStream())
      using (var aes = new AesManaged
      {
        KeySize = KeyBitSize,
        BlockSize = BlockBitSize,
        Mode = CipherMode.CBC,
        Padding = PaddingMode.PKCS7
      })
      {
        // Use random IV.

        aes.GenerateIV();

        cipherStream.Write(aes.IV, 0, aes.IV.Length);

        using (var encrypter = aes.CreateEncryptor(cryptKey, aes.IV))
        {
          using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
          {
            cryptoStream.Write(data, 0, data.Length);
          }

          return cipherStream.ToArray();
        }
      }
    }

    public byte[] Decrypt(byte[] data)
    {
      using (var aes = new AesManaged
      {
        KeySize = KeyBitSize,
        BlockSize = BlockBitSize,
        Mode = CipherMode.CBC,
        Padding = PaddingMode.PKCS7
      })
      {
        // Grab IV from message.
        var iv = new byte[BlockBitSize / 8];

        Array.Copy(data, 0, iv, 0, iv.Length);

        using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
        using (var plainTextStream = new MemoryStream())
        {
          using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
          {
            // Decrypt cipher text from message.

            decrypterStream.Write(
                data,
                iv.Length,
                data.Length - iv.Length
            );
          }

          //Return Plain Text
          return plainTextStream.ToArray();
        }
      }
    }

    private class AESEncryptionProviderFactory : IEncryptionProviderFactory
    {
      public IEncryptionProvider Create(string key)
      {
        return new AESEncryptionProvider(
          SHA2DerivedKeyGenerator.GetDerivedKey(Encoding.UTF8.GetBytes(key), KeyBitSize)
        );
      }
    }
  }
}
