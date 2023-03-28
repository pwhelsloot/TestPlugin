using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AMCS.Encryption
{
  public class EncryptionService : IEncryptionService
  {
    private readonly IEncryptionProvider[] providers;

    public EncryptionService(IList<IEncryptionProvider> providers)
    {
      // Higher ID's imply better encryption.

      this.providers = providers
        .OrderByDescending(p => p.Id.GetValueOrDefault())
        .ToArray();
    }

    public byte[] Encrypt(byte[] data)
    {
      var provider = providers[0];

      var encrypted = provider.Encrypt(data);
      if (!provider.Id.HasValue)
        return encrypted;

      var providerIdentifier = ProviderIdParser.Print(provider.Id.Value);

      var result = new byte[providerIdentifier.Length + encrypted.Length];

      Array.Copy(providerIdentifier, result, providerIdentifier.Length);
      Array.Copy(encrypted, 0, result, providerIdentifier.Length, encrypted.Length);

      return result;
    }

    public byte[] Decrypt(byte[] data)
    {
      int? providerId = ProviderIdParser.Parse(ref data);

      var provider = FindProvider(providerId);

      return provider.Decrypt(data);
    }

    private IEncryptionProvider FindProvider(int? providerId)
    {
      foreach (var provider in providers)
      {
        if (provider.Id == providerId)
          return provider;
      }

      throw new ArgumentException("Cannot decrypt data; unknown encryption provider ID");
    }
  }
}
