using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AMCS.Encryption
{
  public static class EncryptionServiceFactory
  {
    public static IList<IEncryptionProviderFactory> Providers = new ReadOnlyCollection<IEncryptionProviderFactory>(new[]
    {
      LegacyEncryptionProvider.Factory,
      AESThenHMACEncryptionProvider.Factory,
      AESEncryptionProvider.Factory
    });

    public static IEncryptionService Create(string key)
    {
      return new EncryptionService(Providers.Select(p => p.Create(key)).ToList());
    }
  }
}
