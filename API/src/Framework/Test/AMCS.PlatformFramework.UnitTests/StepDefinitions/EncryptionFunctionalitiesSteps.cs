using AMCS.Data.Support.Security;
using AMCS.Encryption;
using AMCS.Encryption.BouncyCastle;
using AMCS.PlatformFramework.UnitTests.TestProperties;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using TechTalk.SpecFlow;
using StringEncryptor = AMCS.PlatformFramework.UnitTests.TestProperties.StringEncryptor;

namespace AMCS.PlatformFrameWork.UnitTests
{
  [Binding]
  public class EncryptionFunctionalitiesSteps
  {
    private const string MODE_BOUNTYCASTLE = "BOUNTYCASTLE";
    private const string MODE_NET_FRAMEWORK = "NETFRAMEWORK";
    private const string MODE_LEGACY = "LEGACY";
    private const string MODE_STRING = "STRING";
    private string InputString;
    private string EncryptedString;
    private string DecryptedString;
    private Mode mode;
    private IEncryptionService EncryptionService;
    private IEncryptionService LegacyEncryptionService;
    private int iterations = 0;
    string key = GetKey();
    [Given(@"an (.*) input string")]
    public void GivenAnHelloWorldInputString(string inputString)
    {
      InputString = inputString;
    }

    [When(@"encryption service (.*) is started")]
    public void WhenEncryptionServiceBouncyCastleIsStarted(string encyptionType)
    {

      switch (encyptionType.ToUpperInvariant())
      {
        case MODE_BOUNTYCASTLE:
          mode = Mode.BouncyCastle;
          break;
        case MODE_NET_FRAMEWORK:
          mode = Mode.NETFramework;
          break;
      }
      EncryptionService = GetEncryptionService(mode);
      EncryptedString = EncryptionService.Encrypt(InputString);
      DecryptedString = EncryptionService.Decrypt(EncryptedString);
    }

    [When(@"encryption runs continuously for a second")]
    public void WhenEncryptionRunsContinuouslyForASecond()
    {
      var stopwatch = Stopwatch.StartNew();
      while (stopwatch.Elapsed < TimeSpan.FromSeconds(1))
      {
        string expected = "Hello world!";
        string encrypted = EncryptionService.Encrypt(expected);
        string decrypted = EncryptionService.Decrypt(encrypted);

        Assert.AreEqual(expected, decrypted);

        iterations++;
      }
    }

    [When(@"(.*) encryption service is used to encrypt")]
    public void WhenLeacyEncryptionServiceIsStarted(string encryptorUsed)
    {
      switch (encryptorUsed.ToUpperInvariant())
      {
        case MODE_STRING:
#pragma warning disable 0618
          key = StringEncryptor.Key;
          EncryptedString = StringEncryptor.Encrypt(InputString);
          break;
        case MODE_LEGACY:
          LegacyEncryptionService = new EncryptionService(
        new[] { new DummyLegacyEncryptionProvider(key) });

          EncryptedString = LegacyEncryptionService.Encrypt(InputString);
          break;
      }
    }

    [When(@"BouncyCastle encryption service is used to decrypt")]
    public void WhenBouncyCastleEncryptionServiceIsStarted()
    {
      EncryptionService = GetEncryptionService(key, Mode.BouncyCastle);
      DecryptedString = EncryptionService.Decrypt(EncryptedString);
    }

    [Then(@"more than (.*) iterations of encryption had happened")]
    public void ThenMoreThanIterationsOfEncryptionHadHappened(int expectedCount)
    {
      Assert.Greater(iterations, expectedCount);
    }

    [Then(@"decrypted value of encrypted matches input string")]
    public void ThenDecryptedValueOfEncryptedMatchesHelloWorldInputString()
    {
      Assert.AreEqual(InputString, DecryptedString);
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
