using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AMCS.Encryption.BouncyCastle;
using NUnit.Framework;

namespace AMCS.Encryption.Test
{
  [TestFixture]
  public class EncryptionFixture
  {
    [TestCase(Mode.BouncyCastle)]
    [TestCase(Mode.NETFramework)]
    public void SimpleRoundtrip(Mode mode)
    {
      var service = GetEncryptionService(mode);

      string expected = "Hello world!";
      string encrypted = service.Encrypt(expected);
      string decrypted = service.Decrypt(encrypted);

      Assert.AreEqual(expected, decrypted);
    }

    [TestCase(Mode.NETFramework)]
    [TestCase(Mode.BouncyCastle)]
    public void RoundtripPerformance(Mode mode)
    {
      var stopwatch = Stopwatch.StartNew();
      var service = GetEncryptionService(mode);
      int iterations = 0;

      while (stopwatch.Elapsed < TimeSpan.FromSeconds(1))
      {
        string expected = "Hello world!";
        string encrypted = service.Encrypt(expected);
        string decrypted = service.Decrypt(encrypted);

        Assert.AreEqual(expected, decrypted);

        iterations++;
      }

      // We're expected to do at least 1000 roundtrips per second.
      Assert.Greater(iterations, 1000);
    }

    [Test]
    public void DecryptLegacy()
    {
      string key = GetKey();

      var legacyService = new EncryptionService(
        new[] { new DummyLegacyEncryptionProvider(key) });

      var service = GetEncryptionService(key, Mode.BouncyCastle);

      string expected = "Hello world!";
      string encrypted = legacyService.Encrypt(expected);
      string decrypted = service.Decrypt(encrypted);

      Assert.AreEqual(expected, decrypted);
    }

    [Test]
    public void DecryptStringEncryptor()
    {
      #pragma warning disable 0618
      string key = StringEncryptor.Key;

      var service = GetEncryptionService(key, Mode.BouncyCastle);

      string expected = "Hello world!";
      string encrypted = StringEncryptor.Encrypt(expected);
      #pragma warning restore 0618
      string decrypted = service.Decrypt(encrypted);

      Assert.AreEqual(expected, decrypted);
    }

    private IEncryptionService GetEncryptionService(Mode mode)
    {
      return GetEncryptionService(GetKey(), mode);
    }

    private static string GetKey()
    {
      return RandomText.CreateKey(40);
    }

    private IEncryptionService GetEncryptionService(string key, Mode mode)
    {
      if (mode == Mode.BouncyCastle)
        return BouncyEncryptionServiceFactory.Create(key);
      return EncryptionServiceFactory.Create(key);
    }

    // We have a copy of the legacy encryption provider here because the real one doesn't
    // support encryption, just to make sure nobody every uses it by accident.
    private class DummyLegacyEncryptionProvider : IEncryptionProvider
    {
      private readonly byte[] key;

      public DummyLegacyEncryptionProvider(string key)
      {
        using (var md5 = new MD5CryptoServiceProvider())
        {
          this.key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
      }

      public int? Id => null;

      public byte[] Encrypt(byte[] data)
      {
        using (var cryptoServiceProvider = new TripleDESCryptoServiceProvider { Key = key, Mode = CipherMode.ECB })
        using (var encryptor = cryptoServiceProvider.CreateEncryptor())
        {
          return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
      }

      public byte[] Decrypt(byte[] data)
      {
        throw new NotSupportedException();
      }
    }

    public enum Mode
    {
      BouncyCastle,
      NETFramework
    }
  }
}
